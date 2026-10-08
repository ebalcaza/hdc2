import { pedir } from "./cliente.js";

export const auth = {
  login: (datos) => pedir("/api/auth/login", { metodo: "POST", cuerpo: datos }),
  registrarIndividuo: (datos) => pedir("/api/auth/registro/individuo", { metodo: "POST", cuerpo: datos }),
  registrarOrganizacion: (datos) => pedir("/api/auth/registro/organizacion", { metodo: "POST", cuerpo: datos })
};

export const reportes = {
  listar: () => pedir("/api/reportes"),
  obtener: (id) => pedir(`/api/reportes/${id}`),
  crear: (datos) => pedir("/api/reportes", { metodo: "POST", cuerpo: datos }),
  actualizar: (id, datos) => pedir(`/api/reportes/${id}`, { metodo: "PUT", cuerpo: datos }),
  eliminar: (id) => pedir(`/api/reportes/${id}`, { metodo: "DELETE" }),
  pdf: (id) => pedir(`/api/reportes/${id}/pdf`, { archivo: true })
};

export const dashboard = {
  obtener: () => pedir("/api/dashboard")
};

export const domicilios = {
  listar: () => pedir("/api/domicilios"),
  crear: (datos) => pedir("/api/domicilios", { metodo: "POST", cuerpo: datos })
};

export const catalogos = {
  fuentes: () => pedir("/api/fuentes-emision"),
  alcances: () => pedir("/api/catalogos/alcances"),
  tiposEmision: () => pedir("/api/catalogos/tipos-emision"),
  unidades: () => pedir("/api/catalogos/unidades")
};

export const admin = {
  estadisticas: (filtros) => pedir("/api/admin/estadisticas", { params: filtros }),

  usuarios: {
    listar: (filtros) => pedir("/api/admin/usuarios", { params: filtros }),
    obtener: (id) => pedir(`/api/admin/usuarios/${id}`),
    darDeBaja: (id) => pedir(`/api/admin/usuarios/${id}/baja`, { metodo: "POST" }),
    darDeAlta: (id) => pedir(`/api/admin/usuarios/${id}/alta`, { metodo: "POST" }),
    crearAdministrador: (datos) => pedir("/api/admin/usuarios/administradores", { metodo: "POST", cuerpo: datos })
  },

  fuentes: {
    listar: () => pedir("/api/admin/fuentes-emision"),
    crear: (datos) => pedir("/api/admin/fuentes-emision", { metodo: "POST", cuerpo: datos }),
    actualizar: (id, datos) => pedir(`/api/admin/fuentes-emision/${id}`, { metodo: "PUT", cuerpo: datos }),
    eliminar: (id) => pedir(`/api/admin/fuentes-emision/${id}`, { metodo: "DELETE" })
  }
};
