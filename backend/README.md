# Backend - Huella de carbono

Arquitectura en tres capas (API -> BLL -> DAL), .NET 8, Entity Framework Core y SQL Server.

## Puesta en marcha
1. Ejecutar `ajustes_bd.sql` sobre tu base de datos.
2. En `backend.API/appsettings.json` completar `DefaultConnection` y cambiar `Jwt:Clave`.
3. Crear la solución (opcional) y ejecutar:
   ```
   dotnet new sln -n backend
   dotnet sln add backend.API backend.BLL backend.DAL
   dotnet run --project backend.API
   ```
4. Abrir `/swagger`: registrar un usuario en `/api/auth/registro/individuo`, copiar el token y pegarlo en "Authorize".

## Endpoints
| Método | Ruta | Requisito |
|---|---|---|
| POST | /api/auth/login | - |
| POST | /api/auth/registro/individuo, /registro/organizacion | - |
| GET / POST | /api/domicilios | login |
| GET | /api/fuentes-emision | login |
| GET | /api/dashboard | panel principal del usuario: totales, % por alcance y evolución mensual |
| GET | /api/reportes | RFR05 |
| GET | /api/reportes/{id} | RFR06 |
| POST | /api/reportes | RFR01 |
| PUT | /api/reportes/{id} | RFR03 |
| DELETE | /api/reportes/{id} | RFR04 |
| GET | /api/reportes/{id}/pdf | RFR07 (descarga el PDF) |
| GET | /api/admin/estadisticas | RFR02, solo Administrador (ver filtros abajo) |
| GET | /api/catalogos/alcances, /tipos-emision, /unidades | login |
| GET | /api/admin/usuarios | administrador: lista con filtros y paginación |
| GET | /api/admin/usuarios/{id} | administrador: perfil completo y domicilios |
| POST | /api/admin/usuarios/{id}/baja, /alta | administrador: dar de baja o reactivar una cuenta |
| POST | /api/admin/usuarios/administradores | administrador: crear otra cuenta de administrador |
| GET / POST | /api/admin/fuentes-emision | administrador: listar y agregar fuentes con su factor |
| PUT / DELETE | /api/admin/fuentes-emision/{id} | administrador: editar y eliminar |

Cálculo: `CO2 (kg) = consumo x valor del factor de emisión`; el total del reporte es la suma de sus registros.

## Administrador
Para crear la primera cuenta de administrador, completar en `appsettings.json` (o en variables de entorno)
`AdminInicial:Email` y `AdminInicial:Contrasena` (mínimo 8 caracteres). Se crea al iniciar la API si el email no existe.

## Estadísticas del administrador
`GET /api/admin/estadisticas` acepta, todos opcionales: `desde`, `hasta`, `tipoUsuario` (Individuo u Organizacion),
`idFuente`, `idAlcance`, `idTipoEmision`, `provincia` y `ciudad`.
Devuelve el total de CO2, la cantidad de reportes y registros, y el desglose por fuente, alcance, tipo de emisión,
provincia, tipo de usuario y mes, con el porcentaje de cada ítem.

## PDF
Usa QuestPDF con licencia Community; revisar sus condiciones de uso en https://www.questpdf.com/license/

## Reglas de la administración
- Dar de baja una cuenta pone `FECHA_BAJA` y bloquea nuevos inicios de sesión. Un administrador no puede darse de baja a sí mismo.
  Los tokens ya emitidos siguen valiendo hasta que vencen (`Jwt:ExpiraMinutos`).
- Una fuente de emisión que ya tiene registros en reportes no puede eliminarse ni cambiar su factor o su unidad,
  porque el CO2 de un reporte se calcula con el factor vigente. Para un valor nuevo se agrega otra fuente.
- Para agregar fuentes deben estar cargadas las tablas `UNIDAD_MEDIDA` y `TIPO_EMISION`.
