<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <AdminNavBar />
        <main class="flex-1 bg-[var(--color-secondary)] px-6 pb-32 pt-8">
            <section class="relative mx-auto flex min-h-[690px] max-w-6xl justify-center pt-8">
                <div class="absolute left-[7%] right-[5%] top-[175px] h-px bg-[var(--color-purple)]"></div>

                <div class="relative z-10 w-full max-w-180">
                    <h2 class="mb-10 text-[15px] font-medium text-content">
                        Crear Nuevo Usuario
                    </h2>

                    <UserForm />
                </div>
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
import UserForm from "../components/UserForm.vue";

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
