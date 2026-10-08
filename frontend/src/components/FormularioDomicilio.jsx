export const DOMICILIO_VACIO = {
  calle: "",
  altura: "",
  departamento: "",
  piso: "",
  block: "",
  manzana: "",
  ciudad: "",
  distrito: "",
  provincia: "",
  codigoPostal: "",
  telefono: ""
};

/** Campos del domicilio. Se usa en el registro y en la pantalla de domicilios. */
export default function FormularioDomicilio({ valor, alCambiar }) {
  const cambiar = (campo) => (e) => alCambiar({ ...valor, [campo]: e.target.value });

  const campo = (clave, etiqueta, opciones = {}) => (
    <label className="campo">
      <span>{etiqueta}{opciones.requerido ? " *" : ""}</span>
      <input
        value={valor[clave]}
        onChange={cambiar(clave)}
        required={opciones.requerido}
        maxLength={opciones.max}
        inputMode={opciones.modo}
        type={opciones.tipo ?? "text"}
      />
    </label>
  );

  return (
    <div className="grilla-form">
      {campo("calle", "Calle", { requerido: true, max: 100 })}
      {campo("altura", "Altura", { requerido: true, max: 20 })}
      {campo("departamento", "Departamento", { max: 20 })}
      {campo("piso", "Piso", { max: 20 })}
      {campo("block", "Block", { max: 20 })}
      {campo("manzana", "Manzana", { max: 20 })}
      {campo("ciudad", "Ciudad", { requerido: true, max: 200 })}
      {campo("distrito", "Distrito", { requerido: true, max: 150 })}
      {campo("provincia", "Provincia", { requerido: true, max: 100 })}
      {campo("codigoPostal", "Código postal", { requerido: true, max: 10 })}
      {campo("telefono", "Teléfono", { max: 20, tipo: "tel", modo: "tel" })}
    </div>
  );
}
