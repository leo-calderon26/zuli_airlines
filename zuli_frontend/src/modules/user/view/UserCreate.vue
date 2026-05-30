<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <AdminNavBar />
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
                        <div class="md:col-span-2">
                            <AppInput v-model="form.nationalId" label="Cédula" maxlength="9" placeholder="Ej. 123456789" :error="errors.fields.nationalId" />
                        </div>

                        <AppInput v-model="form.firstName" label="Primer Nombre" maxlength="50" placeholder="Ej. Jonathan" :error="errors.fields.firstName" />

                        <AppInput v-model="form.firstLastName" label="Primer Apellido" maxlength="50" placeholder="Ej. Smith" :error="errors.fields.firstLastName" />

                        <div class="md:col-span-2">
                            <AppInput v-model="form.secondLastName" label="Segundo Apellido" maxlength="50" placeholder="Ej. Alexander" :error="errors.fields.secondLastName" />
                        </div>
                    </div>

                    <div class="mt-5">
                        <AppInput v-model="form.businessEmail" label="Correo" placeholder="j.smith@zuliairlines.com" type="email" :error="errors.fields.businessEmail" />
                    </div>

                    <div class="mt-5">
                        <label class="mb-2 block text-sm font-medium text-[var(--color-content)]">rol</label>
                        <select
                            v-model="form.userRole"
                            class="h-10 w-full rounded-md border border-[var(--color-secondary)] bg-[var(--color-body)] px-4 text-[15px] text-[var(--color-content)] outline-none transition hover:cursor-pointer focus:border-[var(--color-primary)]"
                            :class="{ 'border-error text-error': errors.fields.userRole }"
                        >
                            <option value="" disabled>Asignar Rol de Acceso</option>
                            <option value="Administrator">Administrador</option>
                            <option value="Operator">Operador</option>
                        </select>
                        <p v-if="errors.fields.userRole" class="mt-1 text-sm text-error">{{ errors.fields.userRole }}</p>
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

                <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="errors.fields" />
                <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
            </section>
        </main>
    </div>
</template>

<script setup>
import { reactive } from "vue";
import { useRouter } from "vue-router";
import AdminNavBar from "../../../shared/AdminNavBar.vue";
import ErrorModal from "../../../shared/ErrorModal.vue";
import SuccessModal from "../../../shared/SuccessModal.vue";
import AppButton from "../../../shared/AppButton.vue";
import AppInput from "../../../shared/AppInput.vue";
import { useForm } from "../../../shared/useForm.js";
import userService from "../services/userService";

const router = useRouter();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();

const form = reactive({
    nationalId: "",
    businessEmail: "",
    firstName: "",
    firstLastName: "",
    secondLastName: "",
    userRole: ""
});

function validateForm() {
    clearErrors();

    if (!/^\d{9}$/.test(form.nationalId)) {
        errors.fields.nationalId = "La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones.";
    }

    if (!form.firstName) {
        errors.fields.firstName = "El primer nombre es obligatorio.";
    }

    if (!form.firstLastName) {
        errors.fields.firstLastName = "El primer apellido es obligatorio.";
    }

    if (!form.secondLastName) {
        errors.fields.secondLastName = "El segundo apellido es obligatorio.";
    }

    if (!form.businessEmail) {
        errors.fields.businessEmail = "El correo institucional es obligatorio.";
    } else if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(form.businessEmail)) {
        errors.fields.businessEmail = "El correo institucional no tiene un formato válido.";
    }

    if (!form.userRole) {
        errors.fields.userRole = "Debe seleccionar un rol.";
    }

    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

function clearForm() {
    form.nationalId = "";
    form.businessEmail = "";
    form.firstName = "";
    form.firstLastName = "";
    form.secondLastName = "";
    form.userRole = "";
}

async function createUser() {
    if (!validateForm()) return;

    await handleSubmit(async () => {
        const result = await userService.createUser(form);
        onSuccess(result.message || "Usuario creado correctamente.");
        clearForm();
    }, "No se pudo crear el usuario.");
}

function onSuccessClose() {
    router.push({ path: '/admin/users' });
}
</script>
