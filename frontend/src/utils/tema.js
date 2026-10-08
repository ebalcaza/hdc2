import { useCallback, useState } from "react";

const CLAVE_TEMA = "huella.tema";

/**
 * Tema claro/oscuro. El atributo data-tema de <html> lo fija un script en index.html antes de pintar
 * (lo guardado, o la preferencia del sistema); acá solo se alterna y se recuerda la elección.
 */
export function useTema() {
  const [tema, setTema] = useState(() => document.documentElement.getAttribute("data-tema") || "claro");

  const alternar = useCallback(() => {
    const nuevo = tema === "oscuro" ? "claro" : "oscuro";
    document.documentElement.setAttribute("data-tema", nuevo);
    try {
      localStorage.setItem(CLAVE_TEMA, nuevo);
    } catch {
      /* sin almacenamiento disponible: el cambio vale solo para esta visita */
    }
    setTema(nuevo);
  }, [tema]);

  return { tema, alternar };
}
