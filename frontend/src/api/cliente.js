// Cliente HTTP mínimo: agrega el token, interpreta los errores de la API y avisa si la sesión venció.

const BASE = (import.meta.env.VITE_API_URL ?? "").replace(/\/$/, "");
const CLAVE_SESION = "huella.sesion";

export class ErrorApi extends Error {
  constructor(mensaje, estado) {
    super(mensaje);
    this.estado = estado;
  }
}

// ---- Sesión guardada en el navegador ----
export function leerSesion() {
  try {
    const sesion = JSON.parse(localStorage.getItem(CLAVE_SESION));
    if (!sesion) return null;
    if (new Date(sesion.expiraEn) <= new Date()) {
      localStorage.removeItem(CLAVE_SESION);
      return null;
    }
    return sesion;
  } catch {
    return null;
  }
}

export function guardarSesion(sesion) {
  localStorage.setItem(CLAVE_SESION, JSON.stringify(sesion));
}

export function borrarSesion() {
  localStorage.removeItem(CLAVE_SESION);
}

let alNoAutorizadoFn = () => {};
export function alNoAutorizado(fn) {
  alNoAutorizadoFn = fn;
}

// ---- Errores ----
async function textoDeError(resp) {
  let datos = null;
  try {
    datos = await resp.json();
  } catch {
    /* la respuesta no tenía cuerpo JSON */
  }
  if (datos?.errors) {
    const mensajes = Object.values(datos.errors).flat();
    if (mensajes.length) return mensajes.join(" ");
  }
  return datos?.detail || datos?.title || `Error ${resp.status}`;
}

function nombreDeArchivo(resp) {
  const cabecera = resp.headers.get("Content-Disposition") ?? "";
  const coincidencia = cabecera.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i);
  return coincidencia ? decodeURIComponent(coincidencia[1]) : null;
}

// ---- Pedido genérico ----
export async function pedir(ruta, { metodo = "GET", cuerpo, params, archivo = false } = {}) {
  const sesion = leerSesion();
  const url = new URL(BASE + ruta, window.location.origin);

  if (params) {
    Object.entries(params).forEach(([clave, valor]) => {
      if (valor !== "" && valor !== null && valor !== undefined) url.searchParams.set(clave, valor);
    });
  }

  const cabeceras = {};
  if (cuerpo !== undefined) cabeceras["Content-Type"] = "application/json";
  if (sesion) cabeceras.Authorization = `Bearer ${sesion.token}`;

  let resp;
  try {
    resp = await fetch(url, {
      method: metodo,
      headers: cabeceras,
      body: cuerpo !== undefined ? JSON.stringify(cuerpo) : undefined
    });
  } catch {
    throw new ErrorApi("No se pudo conectar con el servidor.", 0);
  }

  if (resp.status === 401 && sesion) {
    alNoAutorizadoFn();   // el token venció o no es válido: se cierra la sesión
  }

  if (!resp.ok) throw new ErrorApi(await textoDeError(resp), resp.status);
  if (resp.status === 204) return null;
  if (archivo) return { blob: await resp.blob(), nombre: nombreDeArchivo(resp) };
  return resp.json();
}
