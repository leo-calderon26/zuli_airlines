<template>
    <div class="admin-page">
        <PublicNavBar />

        <main class="page-content">
            <section class="form-shell">
                <form class="user-form" @submit.prevent="createUser">
                    <h2 class="form-title">
                        Crear Nuevo Usuario
                    </h2>

                    <div class="form-grid">
                        <div class="field">
                            <label>Cédula</label>
                            <input
                                v-model.trim="form.nationalId"
                                maxlength="9"
                                placeholder="Ej. 123456789"
                                type="text"
                            />
                        </div>

                        <div class="field">
                            <label>Primer Nombre</label>
                            <input
                                v-model.trim="form.firstName"
                                maxlength="50"
                                placeholder="Ej. Jonathan"
                                type="text"
                            />
                        </div>

                        <div class="field">
                            <label>Segundo Apellido</label>
                            <input
                                v-model.trim="form.secondLastName"
                                maxlength="50"
                                placeholder="Ej. Alexander"
                                type="text"
                            />
                        </div>

                        <div class="field">
                            <label>Primer Apellido</label>
                            <input
                                v-model.trim="form.firstLastName"
                                maxlength="50"
                                placeholder="Ej. Smith"
                                type="text"
                            />
                        </div>
                    </div>

                    <div class="field field-full">
                        <label>Correo</label>
                        <input
                            v-model.trim="form.businessEmail"
                            placeholder="j.smith@zuliairlines.com"
                            type="email"
                        />
                    </div>

                    <div class="field field-full">
                        <label>rol</label>
                        <select v-model="form.userRole">
                            <option value="" disabled>Asignar Rol de Acceso</option>
                            <option value="Administrator">Administrador</option>
                            <option value="Operator">Operador</option>
                        </select>
                    </div>

                    <p v-if="errorMessage" class="error-message">
                        {{ errorMessage }}
                    </p>

                    <p v-if="successMessage" class="success-message">
                        {{ successMessage }}
                    </p>

                    <div class="form-actions">
                        <button
                            class="cancel-button"
                            type="button"
                            @click="cancel"
                        >
                            Cancelar
                        </button>

                        <button
                            class="submit-button"
                            :disabled="isLoading"
                            type="submit"
                        >
                            {{ isLoading ? "Creando..." : "Crear Usuario" }}
                        </button>
                    </div>
                </form>
            </section>
        </main>

        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import PublicBottomBar from "../../../shared/PublicBottomBar.vue";
import userService from "../services/userService";

export default {
    name: "UserCreate",

    components: {
        PublicNavBar,
        PublicBottomBar
    },

    data() {
        return {
            form: {
                nationalId: "",
                businessEmail: "",
                firstName: "",
                firstLastName: "",
                secondLastName: "",
                userRole: ""
            },
            isLoading: false,
            errorMessage: "",
            successMessage: ""
        };
    },

    methods: {
        async createUser() {
            this.errorMessage = "";
            this.successMessage = "";

            const validationError = this.validateForm();

            if (validationError) {
                this.errorMessage = validationError;
                return;
            }

            this.isLoading = true;

            try {
                const result = await userService.createUser(this.form);

                this.successMessage = result.message || "Usuario creado correctamente.";
                this.clearForm();
            } catch (error) {
                this.errorMessage = error.message || "No se pudo crear el usuario.";
            } finally {
                this.isLoading = false;
            }
        },

        validateForm() {
            if (!/^\d{9}$/.test(this.form.nationalId)) {
                return "La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones.";
            }

            if (!this.form.firstName) {
                return "El primer nombre es obligatorio.";
            }

            if (!this.form.firstLastName) {
                return "El primer apellido es obligatorio.";
            }

            if (!this.form.secondLastName) {
                return "El segundo apellido es obligatorio.";
            }

            if (!this.form.businessEmail) {
                return "El correo institucional es obligatorio.";
            }

            if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(this.form.businessEmail)) {
                return "El correo institucional no tiene un formato válido.";
            }

            if (!this.form.userRole) {
                return "Debe seleccionar un rol.";
            }

            return "";
        },

        clearForm() {
            this.form = {
                nationalId: "",
                businessEmail: "",
                firstName: "",
                firstLastName: "",
                secondLastName: "",
                userRole: ""
            };
        },

        cancel() {
            this.$router.push({ name: "users" });
        }
    }
};
</script>

<style scoped>
.admin-page {
    min-height: 100svh;
    background: var(--color-secondary);
    display: flex;
    flex-direction: column;
}

.page-content {
    flex: 1;
    padding: 32px 24px 140px;
    background: var(--color-secondary);
}

.form-shell {
    position: relative;
    min-height: 690px;
    display: flex;
    justify-content: center;
    align-items: flex-start;
    padding-top: 32px;
}

.form-shell::before {
    content: "";
    position: absolute;
    top: 175px;
    left: 7%;
    right: 5%;
    height: 1px;
    background: var(--color-purple);
}

.user-form {
    position: relative;
    z-index: 1;
    width: 100%;
    max-width: 720px;
    background: var(--color-white);
    border-radius: 6px;
    padding: 36px;
    box-shadow: 0 3px 8px rgb(0 0 0 / 12%);
}

.form-title {
    margin-bottom: 38px;
    color: var(--color-content);
    font-size: 15px;
    font-weight: 500;
}

.form-grid {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 22px;
}

.field {
    display: flex;
    flex-direction: column;
}

.field-full {
    margin-top: 22px;
}

.field label {
    margin-bottom: 8px;
    color: var(--color-content);
    font-size: 15px;
}

.field input,
.field select {
    height: 40px;
    width: 100%;
    border-radius: 6px;
    border: 1px solid color-mix(in srgb, var(--color-primary) 25%, var(--color-body));
    background: var(--color-body);
    color: var(--color-content);
    padding: 0 16px;
    font-size: 15px;
    outline: none;
    transition: border-color 0.2s ease, filter 0.2s ease;
}

.field input::placeholder {
    color: color-mix(in srgb, var(--color-content) 45%, white);
}

.field input:focus,
.field select:focus {
    border-color: var(--color-primary);
}

.field select:hover {
    cursor: pointer;
}

.error-message,
.success-message {
    margin-top: 22px;
    padding: 12px 16px;
    border-radius: 6px;
    text-align: center;
    font-size: 14px;
    font-weight: 700;
}

.error-message {
    border: 1px solid var(--color-error);
    background: color-mix(in srgb, var(--color-error) 10%, white);
    color: var(--color-error);
}

.success-message {
    border: 1px solid var(--color-gold);
    background: color-mix(in srgb, var(--color-gold) 10%, white);
    color: var(--color-content);
}

.form-actions {
    margin-top: 28px;
    padding-top: 36px;
    border-top: 1px solid var(--color-secondary);
    display: flex;
    justify-content: center;
    gap: 16px;
}

.cancel-button,
.submit-button {
    height: 44px;
    border-radius: 999px;
    font-weight: 500;
    transition: filter 0.2s ease, opacity 0.2s ease;
}

.cancel-button {
    width: 136px;
    border: 1px solid var(--color-primary);
    background: var(--color-surface);
    color: var(--color-primary);
}

.submit-button {
    width: 168px;
    border: none;
    background: var(--color-primary);
    color: white;
    box-shadow: 0 4px 8px rgb(0 0 0 / 20%);
}

.cancel-button:hover,
.submit-button:hover {
    cursor: pointer;
    filter: brightness(0.75);
}

.submit-button:disabled {
    cursor: not-allowed;
    opacity: 0.6;
}

@media (max-width: 760px) {
    .form-grid {
        grid-template-columns: 1fr;
    }

    .user-form {
        padding: 28px;
    }
}
</style>