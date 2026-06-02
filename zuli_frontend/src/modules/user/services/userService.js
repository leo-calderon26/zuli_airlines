const userService = {
    async createUser(userData) {
        const response = await fetch("/api/admin/users", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify({
                nationalId: userData.nationalId,
                businessEmail: userData.businessEmail,
                firstName: userData.firstName,
                firstLastName: userData.firstLastName,
                secondLastName: userData.secondLastName,
                userRole: userData.userRole
            })
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildRequestError(response, data);
        }

        return data;
    },

    async updateUser(userId, userData) {
        const response = await fetch(`/api/admin/users/${userId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify({
                nationalId: userData.nationalId,
                businessEmail: userData.businessEmail,
                firstName: userData.firstName,
                firstLastName: userData.firstLastName,
                secondLastName: userData.secondLastName,
                userRole: userData.userRole
            })
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildRequestError(response, data);
        }

        return data;
    },

    async getUsers(filters) {
        const query = new URLSearchParams({
            searchType: filters.searchType,
            search: filters.search,
            page: filters.page,
            pageSize: filters.pageSize
        });

        const response = await fetch(`/api/admin/users?${query.toString()}`, {
            method: "GET",
            credentials: "include"
        });

        const data = await readResponseBody(response);

        if (!response.ok) {
            throw buildRequestError(response, data);
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

function buildRequestError(response, data) {
    const message =
        getValidationErrorMessage(data) ||
        data.message ||
        data.detail ||
        getDefaultErrorMessage(response.status);

    const error = new Error(message);
    error.status = response.status;
    error.data = data;

    return error;
}

function getValidationErrorMessage(data) {
    if (!data || !data.errors) {
        return null;
    }

    const errorKeys = Object.keys(data.errors);

    if (errorKeys.length === 0) {
        return null;
    }

    const firstError = data.errors[errorKeys[0]];

    if (Array.isArray(firstError) && firstError.length > 0) {
        return firstError[0];
    }

    return null;
}

function getDefaultErrorMessage(status) {
    if (status === 400) {
        return "Revise los datos ingresados.";
    }

    if (status === 401) {
        return "Debe iniciar sesión para continuar.";
    }

    if (status === 403) {
        return "No tiene permisos para consultar usuarios.";
    }

    if (status === 429) {
        return "Demasiados intentos. Intente más tarde.";
    }

    if (status >= 500) {
        return "Ocurrió un error en el servidor. Intente de nuevo más tarde.";
    }

    return "No se pudo completar la solicitud.";
}

export default userService;