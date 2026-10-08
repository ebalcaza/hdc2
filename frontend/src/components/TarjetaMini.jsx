/** Mini-card con un dato destacado. `variante` (1 a 6) elige el degradé pastel de fondo. */
export default function TarjetaMini({ variante = 1, icono, titulo, valor, detalle, pie }) {
  return (
    <article className={`mini-card mini-${variante}`}>
      <div className="mini-cabecera">
        <span className="mini-icono" aria-hidden="true">{icono}</span>
        <span>{titulo}</span>
      </div>
      <strong className="mini-valor">{valor}</strong>
      {detalle && <span className="mini-detalle">{detalle}</span>}
      {pie}
    </article>
  );
}
