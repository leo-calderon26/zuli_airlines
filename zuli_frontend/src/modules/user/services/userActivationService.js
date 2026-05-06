const userActivationService = {
    async activateUser(activationData) {
        const response = await fetch("/api/users/activation", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                token: activationData.token,
                password: activationData.password,
                confirmPassword: activationData.confirmPassword
            })
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
        return "El enlace de activación no es válido.";
    }

    if (status === 429) {
        return "Demasiados intentos. Intente más tarde.";
    }

    if (status >= 500) {
        return "Ocurrió un error en el servidor. Intente de nuevo más tarde.";
    }

    return "No se pudo completar la solicitud.";
}

export default userActivationService;