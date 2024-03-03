// Gestión de autenticación y usuarios
const authService = {
    apiUrl: '/api',  // Actualizado para usar la ruta relativa a través del proxy Nginx
    
    // Guardar información del token en localStorage
    setAuthData: function(data) {
        localStorage.setItem('token', data.token);
        localStorage.setItem('userId', data.userId);
        localStorage.setItem('userName', data.userName);
        localStorage.setItem('userEmail', data.userEmail);
        localStorage.setItem('userRole', data.rol);
        localStorage.setItem('tokenExpiry', new Date().getTime() + (data.expiresIn * 1000));
    },
    
    // Borrar información de autenticación
    clearAuthData: function() {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userRole');
        localStorage.removeItem('tokenExpiry');
    },
    
    // Verificar si el usuario está autenticado
    isAuthenticated: function() {
        const token = localStorage.getItem('token');
        const expiry = localStorage.getItem('tokenExpiry');
        
        if (!token || !expiry) {
            return false;
        }
        
        // Comprobar si el token ha expirado
        return new Date().getTime() < parseInt(expiry);
    },
    
    // Verificar si el usuario tiene rol de administrador
    isAdmin: function() {
        return this.isAuthenticated() && localStorage.getItem('userRole') === '1';
    },
    
    // Obtener el token actual
    getToken: function() {
        return localStorage.getItem('token');
    },
    
    // Obtener información del usuario actual
    getCurrentUser: function() {
        if (!this.isAuthenticated()) {
            return null;
        }
        
        return {
            id: localStorage.getItem('userId'),
            name: localStorage.getItem('userName'),
            email: localStorage.getItem('userEmail'),
            role: localStorage.getItem('userRole')
        };
    },
    
    // Iniciar sesión
    login: function(email, password) {
        return $.ajax({
            url: `${this.apiUrl}/auth/login`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                email: email,
                password: password
            })
        });
    },
    
    // Registrar un nuevo usuario
    register: function(userData) {
        return $.ajax({
            url: `${this.apiUrl}/auth/register`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(userData)
        });
    },
    
    // Configurar token para peticiones Ajax
    setupAjaxToken: function() {
        $.ajaxSetup({
            beforeSend: function(xhr) {
                if (authService.isAuthenticated()) {
                    xhr.setRequestHeader('Authorization', `Bearer ${authService.getToken()}`);
                }
            }
        });
    }
};

// Configurar token para todas las peticiones Ajax
$(document).ready(function() {
    authService.setupAjaxToken();
});