# Proyecto de Gestión de Productos

## Pasos para Levantar el Proyecto

### 1. Requisitos Previos
- Docker y Docker Compose instalados
- Git instalado
- Puertos disponibles:
  - 8081 (Frontend)
  - 5001 (API)
  - 1434 (SQL Server)

### 2. Clonar el Repositorio
```bash
git clone https://github.com/nakamavg/ProductManager-API.git
cd ProductManager-API
```

### 3. Configurar el Entorno
El proyecto usa variables de entorno que ya están configuradas en el docker-compose.yml. Si necesitas personalizarlas, puedes modificar los siguientes valores:

- JWT_SECRET_KEY
- JWT_ISSUER_TOKEN
- JWT_AUDIENCE_TOKEN
- JWT_EXPIRE_MINUTES
- ConnectionStrings__DefaultConnection

### 4. Iniciar los Servicios
```bash
docker compose up -d
```

Este comando iniciará tres servicios:
- Base de datos SQL Server (Azure SQL Edge)
- API ASP.NET Core
- Servidor web Nginx (Frontend)

### 5. Verificar la Instalación
Espera aproximadamente 30-60 segundos para que SQL Server se inicialice completamente y ejecuta:
```bash
docker compose ps
```

Deberías ver los tres servicios en estado "running".

### 6. Acceder a la Aplicación

1. Frontend: http://localhost:8081
2. API: http://localhost:5001
3. Base de datos: localhost:1434

### 7. Credenciales por Defecto

Administrador:
- Email: admin@test.com
- Password: admin123

Usuario regular:
- Email: usuario@test.com
- Password: usuario123

### 8. Verificar el Estado de los Servicios

Para ver los logs de los servicios:
```bash
# Todos los servicios
docker compose logs

# Servicio específico
docker compose logs api
docker compose logs sqlserver
docker compose logs web
```

### 9. Detener los Servicios
```bash
docker compose down
```

Para eliminar también los volúmenes (esto borrará los datos de la base de datos):
```bash
docker compose down -v
```

## Solución de Problemas Comunes

### Error de Conexión a la Base de Datos
1. Verifica que el puerto 1434 esté disponible
2. Espera 30-60 segundos después de iniciar los contenedores
3. Revisa los logs: `docker compose logs sqlserver`

### Error al Acceder al Frontend
1. Verifica que el puerto 8081 esté disponible
2. Revisa los logs: `docker compose logs web`
3. Asegúrate de que la API esté funcionando

### Error en la API
1. Verifica que el puerto 5001 esté disponible
2. Revisa los logs: `docker compose logs api`
3. Verifica la conexión con la base de datos

## Monitoreo y Mantenimiento

### Scripts de Mantenimiento
Los scripts están en la carpeta `scripts/`:
```bash
# Ejecutar backup
./scripts/maintenance/backup.sh

# Verificar estado del sistema
./scripts/monitoring/health_check.sh
```

### Logs del Sistema
Los logs se almacenan en:
- Logs de la API: `ProductosAPI/Logs/`
- Logs de auditoría: `logs/audit/`

## Documentación Adicional
Para más información, consulta:
- [API y Endpoints](docs/API.md)
- [Arquitectura del Sistema](docs/ARQUITECTURA.md)
- [Guía de Despliegue](docs/DEPLOYMENT.md)

## Documentación Detallada

- [API y Endpoints](docs/API.md)
- [Arquitectura del Sistema](docs/ARQUITECTURA.md)
- [Modelos y Base de Datos](docs/MODELOS.md)
- [Frontend](docs/FRONTEND.md)
- [Guía de Despliegue](docs/DEPLOYMENT.md)

## Scripts de Mantenimiento

Los scripts de mantenimiento y monitoreo se encuentran en el directorio `scripts/`:
- Backup automatizado: `scripts/maintenance/backup.sh`
- Monitoreo de salud: `scripts/monitoring/health_check.sh`
- Configuración de tareas programadas: `scripts/crontab.example`

## Estructura del Proyecto

- `ProductosAPI.Core/`: API principal en ASP.NET Core
- `Frontend/`: Interfaz de usuario
- `docs/`: Documentación detallada
- `scripts/`: Scripts de mantenimiento y monitoreo

## Contacto y Soporte

Para reportar problemas o solicitar ayuda:
1. Abrir un issue en el repositorio
2. Contactar al equipo de desarrollo

## Descripción General
Este proyecto implementa una API REST para la gestión de productos con autenticación JWT, utilizando una arquitectura moderna y siguiendo las mejores prácticas de desarrollo. El sistema está containerizado usando Docker y sigue una estructura modular y mantenible.

## Tecnologías Utilizadas

### Backend
- **ASP.NET Core**: Elegí esta tecnología por su alto rendimiento, cross-platform y su excelente soporte para APIs REST.
- **Entity Framework Core**: ORM utilizado para la capa de datos, facilitando las operaciones con la base de datos y manteniendo el código limpio y mantenible.
- **SQL Server (Azure SQL Edge)**: Base de datos robusta y compatible con ARM64, perfecta para desarrollo local y producción.
- **JWT (JSON Web Tokens)**: Para la autenticación y autorización, proporcionando una solución stateless y segura.

### Frontend
- **HTML5/CSS3/JavaScript**: Stack frontend básico pero efectivo para la interfaz de usuario.
- **Fetch API**: Para las llamadas al backend, usando promesas para un código más limpio y mantenible.

### Infraestructura
- **Docker**: Containerización de la aplicación para garantizar consistencia entre entornos.
- **Nginx**: Servidor web ligero y eficiente para servir el frontend y como reverse proxy.
- **Docker Compose**: Orquestación de contenedores para desarrollo local.

## Patrones de Diseño y Decisiones Arquitectónicas

### 1. Arquitectura en Capas
He implementado una arquitectura en capas clara:
- **Capa de Presentación**: Controllers y DTOs
- **Capa de Servicios**: Lógica de negocio
- **Capa de Datos**: Contexto de EF Core y modelos
- **Capa de Utilidades**: Servicios transversales como autenticación y logging

Esta separación permite:
- Mayor mantenibilidad
- Testing más sencillo
- Separación clara de responsabilidades

### 2. Patrón Repository (implícito en EF Core)
Aunque no implementé un repository pattern explícito, EF Core ya proporciona una abstracción similar con DbContext, permitiendo:
- Centralizar la lógica de acceso a datos
- Facilitar el testing con bases de datos en memoria
- Mantener el código limpio y DRY

### 3. DTO Pattern
Utilizo DTOs (Data Transfer Objects) para:
- Separar los modelos de dominio de los contratos de API
- Controlar qué datos se exponen al cliente
- Optimizar la transferencia de datos

### 4. Dependency Injection
Uso extensivo de inyección de dependencias para:
- Desacoplamiento de componentes
- Facilitar el testing
- Mejor mantenibilidad
- Código más limpio y testeable

### 5. Servicios Transversales

#### Auditoría
Implementé un sistema de auditoría para:
- Registrar cambios importantes
- Mantener trazabilidad
- Cumplir requisitos de seguridad

#### Autorización basada en roles
Sistema de roles para:
- Control granular de acceso
- Seguridad mejorada
- Flexibilidad en permisos

## Seguridad

### JWT Implementation
- Tokens con tiempo de expiración configurable
- Secretos seguros en variables de entorno
- Validación robusta de tokens

### Filtros de Autorización
Implementación de filtros personalizados para:
- Validar tokens JWT
- Verificar roles y permisos
- Manejar errores de autorización

## Decisiones de Container

### SQL Server (Azure SQL Edge)
Elegí Azure SQL Edge porque:
- Compatible con ARM64 (Apple Silicon)
- Menor huella de memoria
- Perfecto para desarrollo local

### Nginx Configuration
Configuré Nginx para:
- Servir archivos estáticos del frontend
- Actuar como reverse proxy para la API
- Manejar CORS y headers de seguridad

## Mejores Prácticas Implementadas

1. **Logging Centralizado**: Implementación de sistema de logs para debugging y monitoreo.
2. **Manejo Global de Excepciones**: Filtro personalizado para manejar errores de forma consistente.
3. **Validación de Modelos**: Validación robusta en DTOs y modelos.
4. **Configuración Externalizada**: Variables de entorno y archivos de configuración.
5. **Health Checks**: Endpoints de health check para monitoreo.

## Estructura del Proyecto
La estructura del proyecto está organizada por responsabilidades, facilitando:
- Navegación intuitiva del código
- Separación clara de conceptos
- Mantenibilidad mejorada

## Conclusiones
Este proyecto demuestra una implementación moderna de una API REST, con énfasis en:
- Seguridad
- Mantenibilidad
- Escalabilidad
- Buenas prácticas de desarrollo

Las decisiones tomadas están orientadas a crear una base sólida y extensible para futuras funcionalidades.