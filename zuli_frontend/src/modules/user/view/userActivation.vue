<template>
    <div class="activation-page">
        <PublicNavBar />

        <main class="activation-content">
            <form class="activation-card" @submit.prevent="activateUser">
                <div class="activation-header">
                    <h1>Activar Cuenta</h1>
                </div>

                <div class="activation-body">
                    <div class="input-row">
                        <div class="icon-box">
                            <svg class="lock-icon" viewBox="0 0 24 24" fill="currentColor">
                                <path d="M17 8h-1V6a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V10a2 2 0 0 0-2-2Zm-7-2a2 2 0 0 1 4 0v2h-4V6Zm7 14H7V10h10v10Z" />
                            </svg>
                        </div>

                        <div class="field">
                            <label>Contraseña</label>
                            <input
                                v-model="form.password"
                                type="password"
                            />
                        </div>
                    </div>

                    <div class="input-row">
                        <div class="icon-box">
                            <svg class="lock-icon" viewBox="0 0 24 24" fill="currentColor">
                                <path d="M17 8h-1V6a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V10a2 2 0 0 0-2-2Zm-7-2a2 2 0 0 1 4 0v2h-4V6Zm7 14H7V10h10v10Z" />
                            </svg>
                        </div>

                        <div class="field">
                            <label>Confirmar Contraseña</label>
                            <input
                                v-model="form.confirmPassword"
                                type="password"
                            />
                        </div>
                    </div>

                    <p v-if="errorMessage" class="error-message">
                        {{ errorMessage }}
                    </p>

                    <p v-if="successMessage" class="success-message">
                        {{ successMessage }}
                    </p>

                    <div class="button-container">
                        <button
                            class="activate-button"
                            :disabled="isLoading"
                            type="submit"
                        >
                            {{ isLoading ? "Activando..." : "Activar" }}
                        </button>
                    </div>
                </div>
            </form>
        </main>

        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import PublicBottomBar from "../../../shared/PublicBottomBar.vue";
import userActivationService from "../services/userActivationService";

export default {
    name: "UserActivation",

    components: {
        PublicNavBar,
        PublicBottomBar
    },

    data() {
        return {
            form: {
                password: "",
                confirmPassword: ""
            },
            token: "",
            isLoading: false,
            errorMessage: "",
            successMessage: ""
        };
    },

    mounted() {
        this.token = this.$route.query.token || "";

        if (!this.token) {
            this.errorMessage = "El enlace de activación no contiene un token válido.";
        }
    },

    methods: {
        async activateUser() {
            this.errorMessage = "";
            this.successMessage = "";

            const validationError = this.validateForm();

            if (validationError) {
                this.errorMessage = validationError;
                return;
            }

            this.isLoading = true;

            try {
                const result = await userActivationService.activateUser({
                    token: this.token,
                    password: this.form.password,
                    confirmPassword: this.form.confirmPassword
                });

                this.successMessage = result.message || "Cuenta activada correctamente.";

                setTimeout(() => {
                    this.$router.push({ name: "administrativo" });
                }, 1200);
            } catch (error) {
                this.errorMessage = error.message || "No se pudo activar la cuenta.";
            } finally {
                this.isLoading = false;
            }
        },

        validateForm() {
            if (!this.token) {
                return "El enlace de activación no contiene un token válido.";
            }

            if (!this.form.password) {
                return "La contraseña es obligatoria.";
            }

            if (this.form.password.length < 12) {
                return "La contraseña debe tener al menos 12 caracteres.";
            }

            if (!/[A-Z]/.test(this.form.password)) {
                return "La contraseña debe incluir al menos una letra mayúscula.";
            }

            if (!/[a-z]/.test(this.form.password)) {
                return "La contraseña debe incluir al menos una letra minúscula.";
            }

            if (!/[0-9]/.test(this.form.password)) {
                return "La contraseña debe incluir al menos un número.";
            }

            if (!/[\W_]/.test(this.form.password)) {
                return "La contraseña debe incluir al menos un carácter especial.";
            }

            if (this.form.password !== this.form.confirmPassword) {
                return "Las contraseñas no coinciden.";
            }

            return "";
        }
    }
};
</script>

<style scoped>
.activation-page {
    min-height: 100svh;
    background: var(--color-secondary);
    display: flex;
    flex-direction: column;
}

.activation-content {
    flex: 1;
    padding: 140px 24px 150px;
    display: flex;
    justify-content: center;
    align-items: flex-start;
}

.activation-card {
    width: 100%;
    max-width: 735px;
    overflow: hidden;
    border-radius: 6px;
    background: var(--color-surface);
    box-shadow: 0 3px 8px rgb(0 0 0 / 8%);
}

.activation-header {
    background: var(--color-primary);
    padding: 22px;
    text-align: center;
}

.activation-header h1 {
    color: white;
    font-size: 28px;
    font-weight: 700;
    letter-spacing: 0.5px;
}

.activation-body {
    padding: 80px 64px;
}

.input-row {
    display: flex;
    align-items: flex-end;
    gap: 20px;
    margin-bottom: 30px;
}

.icon-box {
    width: 48px;
    height: 48px;
    display: flex;
    align-items: center;
    justify-content: center;
}

.lock-icon {
    width: 36px;
    height: 36px;
    color: black;
}

.field {
    flex: 1;
    display: flex;
    flex-direction: column;
}

.field label {
    margin-bottom: 8px;
    color: #a7a7a7;
    font-size: 18px;
    font-weight: 700;
}

.field input {
    height: 48px;
    width: 100%;
    border-radius: 6px;
    border: 1px solid #aaa;
    background: #f7f7f7;
    padding: 0 16px;
    font-size: 16px;
    outline: none;
    transition: border-color 0.2s ease;
}

.field input:focus {
    border-color: var(--color-primary);
}

.error-message,
.success-message {
    margin: 8px 0 28px;
    padding: 12px 16px;
    border-radius: 6px;
    text-align: center;
    font-size: 14px;
    font-weight: 700;
}

.error-message {
    border: 1px solid #fecaca;
    background: #fef2f2;
    color: #b91c1c;
}

.success-message {
    border: 1px solid #bbf7d0;
    background: #f0fdf4;
    color: #15803d;
}

.button-container {
    display: flex;
    justify-content: center;
    margin-top: 56px;
}

.activate-button {
    height: 48px;
    width: 240px;
    border: none;
    border-radius: 8px;
    background: var(--color-primary);
    color: white;
    font-size: 22px;
    font-weight: 700;
    transition: filter 0.2s ease, opacity 0.2s ease;
}

.activate-button:hover {
    cursor: pointer;
    filter: brightness(0.75);
}

.activate-button:disabled {
    cursor: not-allowed;
    opacity: 0.6;
}

@media (max-width: 720px) {
    .activation-body {
        padding: 48px 28px;
    }

    .input-row {
        gap: 12px;
    }
}
</style>