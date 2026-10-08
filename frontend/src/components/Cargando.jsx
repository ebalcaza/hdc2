export default function Cargando({ texto = "Cargando…" }) {
  return <p className="suave" role="status">{texto}</p>;
}
