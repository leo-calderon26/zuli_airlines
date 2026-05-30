const authService = {
    async login(credentials) {
        const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify({
                businessEmail: credentials.businessEmail,
                password: credentials.password
            })
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildAuthError(response, data);
        }

        return data;
    },

    async me() {
        const response = await fetch("/api/auth/me", {
            method: "GET",
            credentials: "include"
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildAuthError(response, data);
        }

        return data;
    },

    async logout() {
        const response = await fetch("/api/auth/logout", {
            method: "POST",
            credentials: "include"
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildAuthError(response, data);
        }

        return data;
    }
};

async function readResponseBody(response) {
    try {
        return await response.json();
    } catch {
        return {};
    }
}

function buildAuthError(response, data) {
    const error = new Error(data.detail || data.message || getDefaultErrorMessage(response.status));

    error.status = response.status;
    error.data = data;

    return error;
}

function getDefaultErrorMessage(status) {
    if (status === 400) {
        return "Solicitud inválida.";
    }

    if (status === 401) {
        return "Correo o contraseña incorrectos.";
    }

    if (status === 429) {
        return "Demasiados intentos. Intente de nuevo más tarde.";
    }

    return "No se pudo completar la solicitud.";
}

export default authService;