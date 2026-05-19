<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <PublicNavBar />
        <UserNavBar/>
        <main class="flex-1 bg-[var(--color-secondary)] px-6 pb-32 pt-8">
            <section class="relative mx-auto flex min-h-[690px] max-w-6xl justify-center pt-8">
                <div class="absolute left-[7%] right-[5%] top-[175px] h-px bg-[var(--color-purple)]"></div>

                <form
                    class="relative z-10 w-full max-w-[720px] rounded-md bg-white px-9 py-9 shadow-md"
                    @submit.prevent="createUser"
                >
                    <h2 class="mb-10 text-[15px] font-medium text-[var(--color-content)]">
                        Crear Nuevo Usuario
                    </h2>

                    <div class="grid grid-cols-1 gap-x-6 gap-y-5 md:grid-cols-2">
                        <div class="flex flex-col">
                            <label class="mb-2 text-[15px] text-[var(--color-content)]">
                                Cédula
                            </label>
                            <input
                                v-model.trim="form.nationalId"
                                class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                maxlength="9"
                                placeholder="Ej. 123456789"
                                type="text"
                            />
                        </div>

                        <div class="flex flex-col">
                            <label class="mb-2 text-[15px] text-[var(--color-content)]">
                                Primer Nombre
                            </label>
                            <input
                                v-model.trim="form.firstName"
                                class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                maxlength="50"
                                placeholder="Ej. Jonathan"
                                type="text"
                            />
                        </div>

                        <div class="flex flex-col">
                            <label class="mb-2 text-[15px] text-[var(--color-content)]">
                                Segundo Apellido
                            </label>
                            <input
                                v-model.trim="form.secondLastName"
                                class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                maxlength="50"
                                placeholder="Ej. Alexander"
                                type="text"
                            />
                        </div>

                        <div class="flex flex-col">
                            <label class="mb-2 text-[15px] text-[var(--color-content)]">
                                Primer Apellido
                            </label>
                            <input
                                v-model.trim="form.firstLastName"
                                class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                                maxlength="50"
                                placeholder="Ej. Smith"
                                type="text"
                            />
                        </div>
                    </div>

                    <div class="mt-5 flex flex-col">
                        <label class="mb-2 text-[15px] text-[var(--color-content)]">
                            Correo
                        </label>
                        <input
                            v-model.trim="form.businessEmail"
                            class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition focus:border-[var(--color-primary)]"
                            placeholder="j.smith@zuliairlines.com"
                            type="email"
                        />
                    </div>

                    <div class="mt-5 flex flex-col">
                        <label class="mb-2 text-[15px] text-[var(--color-content)]">
                            rol
                        </label>
                        <select
                            v-model="form.userRole"
                            class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition hover:cursor-pointer focus:border-[var(--color-primary)]"
                        >
                            <option value="" disabled>Asignar Rol de Acceso</option>
                            <option value="Administrator">Administrador</option>
                            <option value="Operator">Operador</option>
                        </select>
                    </div>

                    <div class="mt-7 border-t border-[var(--color-secondary)] pt-9">
                        <div class="flex justify-center gap-4">
                            <AppButton
                                :loading="isLoading"
                                type="submit"
                                variant="primary"
                            >
                                Crear Usuario
                            </AppButton>
                        </div>
                    </div>
                </form>

                <ErrorModal v-model="showErrorModal" :message="errorMessage" />
                <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
            </section>
        </main>
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import UserNavBar from "../components/UserNavBar.vue";
import ErrorModal from "../../../shared/ErrorModal.vue";
import SuccessModal from "../../../shared/SuccessModal.vue";
import AppButton from "../../../shared/AppButton.vue";
import userService from "../services/userService";

export default {
    name: "UserCreate",

    components: {
        PublicNavBar,
        UserNavBar,
        ErrorModal,
        SuccessModal,
        AppButton
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
            successMessage: "",
            showErrorModal: false,
            showSuccessModal: false,
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
                this.showSuccessModal = true;
            } catch (error) {
                this.errorMessage = error.message || "No se pudo crear el usuario.";
                this.showErrorModal = true;
            } finally {
                this.isLoading = false;
            }
        },

        onSuccessClose() {
            this.showSuccessModal = false;
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
    }
};
</script>