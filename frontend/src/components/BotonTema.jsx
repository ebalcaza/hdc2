import { useTema } from "../utils/tema.js";

/** Alterna entre modo claro y modo oscuro. */
export default function BotonTema({ flotante = false }) {
  const { tema, alternar } = useTema();
  const oscuro = tema === "oscuro";
  const accion = oscuro ? "Cambiar a modo claro" : "Cambiar a modo oscuro";

  return (
    <button
      type="button"
      className={`btn-tema${flotante ? " btn-tema-flotante" : ""}`}
      onClick={alternar}
      aria-label={accion}
      title={accion}
    >
      <span aria-hidden="true">{oscuro ? "☀️" : "🌙"}</span>
      <span className="btn-tema-texto">{oscuro ? "Claro" : "Oscuro"}</span>
    </button>
  );
}
