#!/bin/bash

# Script de monitoreo de salud del sistema
# Uso: ./health_check.sh [slack_webhook_url]

# Configuración
SLACK_WEBHOOK=${1:-""}
API_URL="http://localhost:5001"
NGINX_URL="http://localhost:8081"
LOG_FILE="/var/log/health_check.log"

check_service() {
    local service_name=$1
    local url=$2
    
    echo "Verificando $service_name..."
    if curl -f "$url/health" >/dev/null 2>&1; then
        echo "✅ $service_name: OK"
        return 0
    else
        echo "❌ $service_name: ERROR"
        return 1
    fi
}

check_docker_services() {
    echo "Verificando contenedores Docker..."
    docker compose ps --format "table {{.Name}}\t{{.Status}}"
}

check_resources() {
    echo -e "\nUso de recursos:"
    echo "CPU:"
    top -l 1 | grep "CPU usage"
    echo -e "\nMemoria:"
    vm_stat
    echo -e "\nDisco:"
    df -h
}

send_alert() {
    local message=$1
    if [ -n "$SLACK_WEBHOOK" ]; then
        curl -X POST -H 'Content-type: application/json' \
            --data "{\"text\":\"$message\"}" \
            "$SLACK_WEBHOOK"
    fi
}

# Ejecutar verificaciones
{
    echo "=== Health Check $(date) ==="
    
    # Verificar servicios
    check_service "API" "$API_URL"
    API_STATUS=$?
    
    check_service "Web" "$NGINX_URL"
    WEB_STATUS=$?
    
    # Verificar Docker
    check_docker_services
    
    # Verificar recursos
    check_resources
    
    # Enviar alertas si hay problemas
    if [ $API_STATUS -ne 0 ] || [ $WEB_STATUS -ne 0 ]; then
        send_alert "⚠️ Alerta: Problemas detectados en servicios"
    fi
    
    echo "=== Fin Health Check ==="
} | tee -a "$LOG_FILE"

# Rotar log si es muy grande
if [ -f "$LOG_FILE" ] && [ $(stat -f%z "$LOG_FILE") -gt 5000000 ]; then
    mv "$LOG_FILE" "${LOG_FILE}.1"
fi