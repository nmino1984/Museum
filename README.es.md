# 🏛️ Museum API — Resumen para evaluador

API REST desarrollada como prueba técnica para **Xtra Travel**. Gestiona museos y los artículos que contienen, aplicando Clean Architecture, soft delete y una suite de tests completa.

---

## ✅ Qué cubre del challenge

### Tareas obligatorias completadas

- CRUD completo de **Museos** (crear, listar, editar, eliminar)
- CRUD completo de **Artículos** (crear, listar, editar, eliminar)
- Relación Museo → Artículos (1 a muchos)
- Filtrado de museos por tema (`Art`, `Natural Sciences`, `History`)
- Listado de artículos por museo

### Tareas opcionales completadas

- **Soft delete** (`Remove`): marca el registro como eliminado sin borrarlo físicamente
- **Hard delete** (`Delete`): elimina el registro de la base de datos (bloqueado en museos con artículos activos)
- **BulkRegister**: creación de múltiples artículos en una sola petición (atómica: todo o nada)
- **MarkDamaged**: marca un artículo como dañado
- **RelocateArticle**: mueve un artículo a un museo diferente
- **44 tests automatizados** cubriendo CRUD, validaciones y reglas de negocio

---

## 🛠️ Stack tecnológico

| Elemento | Tecnología |
|---|---|
| Framework | ASP.NET Core 9.0 |
| ORM | Entity Framework Core 9 |
| Base de datos | SQL Server (autenticación Windows) |
| Validación | FluentValidation 11 |
| Mapeo | AutoMapper 13 |
| Documentación | Swagger / Swashbuckle 10 |
| Tests | xUnit + EF Core InMemory |

---

## 🚀 Instalación rápida

```bash
# 1. Clonar el repositorio
git clone https://github.com/nmino1984/Museum.git
cd Museum

# 2. Configurar la cadena de conexión en WebAPI/appsettings.json
#    Clave: "MuseumsDBConnectionString"

# 3. Aplicar migraciones
dotnet ef database update \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project WebAPI/WebAPI.csproj

# 4. Ejecutar la API
dotnet run --project WebAPI/WebAPI.csproj
```

Swagger disponible en `https://localhost:<puerto>/swagger/index.html`.

---

## 🧪 Ejecutar los tests

```bash
dotnet test Test/Test.csproj
```

Los 44 tests usan **EF Core InMemory** — no requieren SQL Server instalado.

Cubren: CRUD de museos y artículos, filtrado por tema, reglas de negocio (bloqueo de borrado, cascade soft-delete), operaciones especiales (MarkDamaged, Relocate, BulkRegister) y validaciones de entrada.

---

## 🔲 Qué queda pendiente

La **interfaz de usuario** (frontend) no forma parte de este repositorio. Está en desarrollo como un proyecto separado y conectará con esta API a través de los endpoints documentados.

---

## 💡 Decisiones técnicas destacadas

**Clean Architecture** — se eligió para mantener el dominio y la lógica de negocio independientes del framework y la base de datos. Facilita el testing sin infraestructura real y permite cambiar el ORM o la BD sin tocar la capa de aplicación.

**44 tests con InMemory DB** — el objetivo fue cubrir cada endpoint y cada regla de negocio sin depender de SQL Server. Esto hace que los tests sean rápidos, portables y ejecutables en cualquier CI.

**Soft delete separado del hard delete** — en lugar de un único `DELETE`, la API distingue `Remove` (lógico, recuperable) y `Delete` (físico, permanente). El `DeleteMuseum` está bloqueado mientras el museo tenga artículos activos, y el `RemoveMuseum` aplica el soft-delete en cascada a todos sus artículos.

**`BaseResponse<T>` como envelope universal** — todas las respuestas siguen la misma estructura (`isSuccess`, `message`, `data`, `errors`), lo que simplifica el consumo desde el frontend y hace predecible el manejo de errores.

---

🇬🇧 [Full English README](README.md)
