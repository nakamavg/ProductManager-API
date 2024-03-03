# Proyecto de Gestión de Productos

## Guía de Inicio Rápido

### Requisitos Previos
- Docker y Docker Compose
- Git

### Instalación y Ejecución

1. Clonar el repositorio:
```bash
git clone <url-repositorio>
cd prueba
```

2. Iniciar los servicios:
```bash
docker compose up -d
```

3. Verificar que los servicios estén funcionando:
```bash
docker compose ps
```

Los servicios estarán disponibles en:
- Frontend: http://localhost:8081
- API: http://localhost:5001
- SQL Server: localhost:1434

### Prueba Rápida

1. **Credenciales por defecto**:

Administrador:
- Email: admin@test.com
- Password: admin123

Usuario regular:
- Email: usuario@test.com
- Password: usuario123

2. **Probar API**:
```bash
# Obtener token (usando credenciales de admin)
curl -X POST http://localhost:5001/api/auth/login \
-H "Content-Type: application/json" \
-d '{"email":"admin@test.com","password":"admin123"}'

# Usar el token para obtener productos
curl http://localhost:5001/api/productos \
-H "Authorization: Bearer {token-recibido}"
```

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

## Solución de Problemas Comunes

1. **Los contenedores no inician**:
```bash
# Verificar logs
docker compose logs

# Reiniciar servicios
docker compose down && docker compose up -d
```

2. **Error de conexión a la base de datos**:
- Verificar que el puerto 1434 esté disponible
- Esperar 30-60 segundos después de iniciar los contenedores para que SQL Server esté listo

3. **Problemas de autenticación**:
- Verificar las credenciales
- Asegurarse de incluir el token en el header "Authorization"

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