<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <PublicNavBar />

        <main class="flex flex-1 items-start justify-center bg-[var(--color-secondary)] px-6 pb-36 pt-[140px]">
            <form
                class="w-full max-w-[735px] overflow-hidden rounded-md bg-white shadow-sm"
                @submit.prevent="activateUser"
            >
                <div class="bg-[var(--color-primary)] px-6 py-6 text-center">
                    <h1 class="text-[28px] font-bold tracking-wide text-white">
                        Activar Cuenta
                    </h1>
                </div>

                <div class="bg-white px-16 py-20">
                    <div class="mb-8 flex items-end gap-5">
                        <div class="flex h-12 w-12 items-center justify-center">
                            <svg
                                class="h-9 w-9 text-[var(--color-content)]"
                                viewBox="0 0 24 24"
                                fill="currentColor"
                            >
                                <path d="M17 8h-1V6a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V10a2 2 0 0 0-2-2Zm-7-2a2 2 0 0 1 4 0v2h-4V6Zm7 14H7V10h10v10Z" />
                            </svg>
                        </div>

                        <div class="flex flex-1 flex-col">
                            <label class="mb-2 text-[18px] font-bold text-gray-400">
                                Contraseña
                            </label>
                            <input
                                v-model="form.password"
                                class="h-12 w-full rounded-md border border-gray-300 bg-[var(--color-body)] px-4 text-[16px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                type="password"
                            />
                        </div>
                    </div>

                    <div class="mb-12 flex items-end gap-5">
                        <div class="flex h-12 w-12 items-center justify-center">
                            <svg
                                class="h-9 w-9 text-[var(--color-content)]"
                                viewBox="0 0 24 24"
                                fill="currentColor"
                            >
                                <path d="M17 8h-1V6a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V10a2 2 0 0 0-2-2Zm-7-2a2 2 0 0 1 4 0v2h-4V6Zm7 14H7V10h10v10Z" />
                            </svg>
                        </div>

                        <div class="flex flex-1 flex-col">
                            <label class="mb-2 text-[18px] font-bold text-gray-400">
                                Confirmar Contraseña
                            </label>
                            <input
                                v-model="form.confirmPassword"
                                class="h-12 w-full rounded-md border border-gray-300 bg-[var(--color-body)] px-4 text-[16px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                type="password"
                            />
                        </div>
                    </div>

                    <div class="mt-14 flex justify-center">
                        <AppButton
                            :loading="isLoading"
                            type="submit"
                            variant="primary"
                            size="lg"
                        >
                            Activar
                        </AppButton>
                    </div>
                </div>
            </form>

            <ErrorModal v-model="showErrorModal" :message="errorMessage" />
            <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
        </main>

        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import PublicBottomBar from "../../../shared/PublicBottomBar.vue";
import ErrorModal from "../../../shared/ErrorModal.vue";
import SuccessModal from "../../../shared/SuccessModal.vue";
import AppButton from "../../../shared/AppButton.vue";
import userActivationService from "../services/userActivationService";

export default {
    name: "UserActivation",

    components: {
        PublicNavBar,
        PublicBottomBar,
        ErrorModal,
        SuccessModal,
        AppButton
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
            successMessage: "",
            showErrorModal: false,
            showSuccessModal: false,
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
                this.showSuccessModal = true;
            } catch (error) {
                this.errorMessage = error.message || "No se pudo activar la cuenta.";
                this.showErrorModal = true;
            } finally {
                this.isLoading = false;
            }
        },

        onSuccessClose() {
            this.$router.push({ name: "administrativo" });
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