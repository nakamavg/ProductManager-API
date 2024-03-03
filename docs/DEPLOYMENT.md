# Guía de Despliegue

## Requisitos del Sistema

### Software Necesario
- Docker y Docker Compose
- Git
- Azure SQL Edge o SQL Server
- Nginx
- .NET SDK 6.0 o superior

### Recursos Recomendados
- CPU: 2 cores mínimo
- RAM: 4GB mínimo
- Almacenamiento: 20GB mínimo
- Red: Puerto 80/443 accesible

## Entornos de Despliegue

### 1. Desarrollo Local
```bash
# Clonar repositorio
git clone <repository-url>
cd producto-api

# Iniciar servicios
docker compose up -d

# Verificar servicios
docker compose ps
```

### 2. Staging
```bash
# Variables de entorno específicas
export ASPNETCORE_ENVIRONMENT=Staging
export ConnectionStrings__DefaultConnection="Server=staging-db;Database=ProductosDB;..."

# Desplegar con configuración de staging
docker compose -f docker-compose.yml -f docker-compose.staging.yml up -d
```

### 3. Producción
```bash
# Variables de entorno de producción
export ASPNETCORE_ENVIRONMENT=Production
export JWT_SECRET_KEY=<production-key>
export ConnectionStrings__DefaultConnection="Server=prod-db;Database=ProductosDB;..."

# Desplegar en producción
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Configuración de Seguridad

### 1. SSL/TLS
- Certificado SSL válido
- Configuración HTTPS en Nginx
- Redirección HTTP a HTTPS

### 2. Secretos y Configuraciones
- Usar Azure Key Vault o similar
- Variables de entorno seguras
- Rotación de secretos

### 3. Hardening
- Firewall configurado
- Puertos mínimos expuestos
- Updates automáticos

## Monitoreo y Mantenimiento

### 1. Logging
- Centralizado en ELK Stack
- Rotación de logs
- Alertas configuradas

### 2. Backups
- Base de datos: Diario
- Configuraciones: Por cambio
- Retención: 30 días

### 3. Métricas
- CPU/Memoria/Disco
- Tiempos de respuesta
- Errores por minuto

## Procedimientos

### 1. Despliegue
1. Backup de datos
2. Pull de cambios
3. Build de contenedores
4. Pruebas de smoke
5. Switch de tráfico

### 2. Rollback
1. Identificar versión estable
2. Restaurar contenedores
3. Verificar servicios
4. Validar datos

### 3. Escalado
1. Ajustar recursos
2. Replicar servicios
3. Balancear carga
4. Monitorear performance

## Troubleshooting

### 1. Problemas Comunes
- Conexión a BD
- Certificados SSL
- Permisos Docker
- Logs de error

### 2. Verificaciones
- Estado de servicios
- Conectividad red
- Espacio en disco
- Logs de aplicación

### 3. Contactos
- DevOps: devops@empresa.com
- BD Admin: dba@empresa.com
- Security: security@empresa.com

## CI/CD Pipeline

### 1. Integración Continua
```mermaid
graph LR
    A[Commit] --> B[Build]
    B --> C[Test]
    C --> D[Análisis]
    D --> E[Artifact]
```

### 2. Despliegue Continuo
```mermaid
graph LR
    A[Artifact] --> B[Staging]
    B --> C[Tests E2E]
    C --> D[Production]
    D --> E[Monitoreo]
```

## Scripts de Mantenimiento

### 1. Backup
```bash
#!/bin/bash
# Backup de BD y configuraciones
DATE=$(date +%Y%m%d)
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U SA -P "$SA_PASSWORD" \
    -Q "BACKUP DATABASE ProductosDB TO DISK = '/var/opt/mssql/backup/productos_$DATE.bak'"
```

### 2. Healthcheck
```bash
#!/bin/bash
# Verificación de servicios
curl -f http://localhost/health
docker compose ps
df -h
free -m
```

### 3. Limpieza
```bash
#!/bin/bash
# Limpieza de logs y temporales
find /var/log -name "*.log" -mtime +30 -delete
docker system prune -f
```

## Optimización de Producción

### 1. Nginx
- Caché de contenido estático
- Compresión gzip
- SSL session cache
- Worker processes optimizados

### 2. SQL Server
- Índices optimizados
- Estadísticas actualizadas
- Plan cache ajustado
- Memory settings optimizados

### 3. API
- Response compression
- Output cache
- Connection pooling
- Distributed cache

## Escalabilidad

### 1. Horizontal
- Load balancer configurado
- Sesiones distribuidas
- Caché distribuido
- DB replication

### 2. Vertical
- CPU/RAM escalable
- Storage expandible
- Network bandwidth
- IOPS optimizados

## Plan de Recuperación

### 1. Disaster Recovery
- Backups verificados
- Procedimiento documentado
- Tiempo objetivo definido
- Ensayos regulares

### 2. Alta Disponibilidad
- Multi-AZ deployment
- Auto-healing
- Health checks
- Failover automático