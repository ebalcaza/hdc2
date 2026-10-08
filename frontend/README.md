# Frontend - Huella de carbono

React 18 + Vite + React Router. Sin librerías de UI ni de gráficos: el diseño es responsivo con CSS propio
(en el celular las tablas se transforman en tarjetas y el formulario de reportes se apila).

## Puesta en marcha
1. Instalar Node.js 18 o superior.
2. Copiar `.env.example` como `.env` y poner en `VITE_API_URL` la dirección de la API
   (el puerto aparece en la consola al ejecutar `dotnet run --project backend.API`).
3. Instalar y ejecutar:
   ```
   npm install
   npm run dev
   ```
4. Abrir http://localhost:5173 (ese origen ya está permitido en `Cors:Origenes` de la API).

Si el navegador no confía en el certificado HTTPS de la API: `dotnet dev-certs https --trust`.

Para probar desde el celular en la misma red, ejecutar `npm run dev -- --host`, agregar la dirección
`http://IP-DE-TU-PC:5173` a `Cors:Origenes` en `appsettings.json` y apuntar `VITE_API_URL` a la IP de la PC.

## Pantallas
| Ruta | Quién | Requisito |
|---|---|---|
| /login, /registro | todos | inicio de sesión y alta de individuo u organización |
| /inicio | individuo, organización | panel principal: totales, % por alcance, curva de evolución |
| /reportes | individuo, organización | RFR05 lista, RFR04 eliminar, RFR07 PDF |
| /reportes/nuevo | individuo, organización | RFR01 |
| /reportes/:id | individuo, organización | RFR06 detalle |
| /reportes/:id/editar | individuo, organización | RFR03 |
| /domicilios | individuo, organización | listar y agregar domicilios |
| /admin/estadisticas | administrador | RFR02 |
| /admin/usuarios | administrador | lista, filtros, baja/alta y alta de administradores |
| /admin/usuarios/:id | administrador | perfil completo de una cuenta |
| /admin/factores | administrador | alta, edición y baja de fuentes y factores de emisión |

## Estructura
```
src/api/         cliente HTTP (token, errores) y funciones por recurso
src/context/     sesión (AuthContext)
src/components/  Layout, rutas protegidas, formulario de domicilio, gráfico de barras
src/pages/       una página por pantalla
src/utils/       formato de números/fechas, descarga de PDF, hook useCarga
```

## Tema claro / oscuro
El botón de la cabecera (y el flotante en login y registro) alterna el tema y recuerda la elección en el navegador.
La primera vez usa la preferencia del sistema. Los colores están en las variables CSS de `src/styles.css`
(`:root` es el tema claro y `:root[data-tema="oscuro"]` el oscuro).
