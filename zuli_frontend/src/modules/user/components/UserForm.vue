<script setup>
import { reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import { useForm } from '../../../shared/useForm.js';
import { useUser } from '../composable/useUser';

const props = defineProps({
    user: {
        type: Object,
        default: null
    },
    isEdit: {
        type: Boolean,
        default: false
    }
});

const router = useRouter();
const { createUser, updateUser } = useUser();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();

const form = reactive({
    userId: '',
    nationalId: '',
    businessEmail: '',
    firstName: '',
    firstLastName: '',
    secondLastName: '',
    userRole: ''
});

function syncForm(user) {
    if (!user) {
        return;
    }

    form.userId = user.userId ?? '';
    form.nationalId = user.nationalId ?? '';
    form.businessEmail = user.businessEmail ?? '';
    form.firstName = user.firstName ?? '';
    form.firstLastName = user.firstLastName ?? '';
    form.secondLastName = user.secondLastName ?? '';
    form.userRole = user.userRole ?? '';
}

watch(
    () => props.user,
    (user) => syncForm(user),
    { immediate: true, deep: true }
);

function validateForm() {
    clearErrors();

    if (!props.isEdit && !/^\d{9}$/.test(form.nationalId)) {
        errors.fields.nationalId = 'La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones.';
    }

    if (!form.firstName.trim()) {
        errors.fields.firstName = 'El primer nombre es obligatorio.';
    }

    if (!form.firstLastName.trim()) {
        errors.fields.firstLastName = 'El primer apellido es obligatorio.';
    }

    if (!form.secondLastName.trim()) {
        errors.fields.secondLastName = 'El segundo apellido es obligatorio.';
    }

    if (!form.businessEmail.trim()) {
        errors.fields.businessEmail = 'El correo institucional es obligatorio.';
    } else if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(form.businessEmail.trim())) {
        errors.fields.businessEmail = 'El correo institucional no tiene un formato válido.';
    }

    if (!form.userRole) {
        errors.fields.userRole = 'Debe seleccionar un rol.';
    }

    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function submit() {
    if (!validateForm()) return;

    await handleSubmit(async () => {
        const payload = {
            nationalId: form.nationalId.trim(),
            businessEmail: form.businessEmail.trim(),
            firstName: form.firstName.trim(),
            firstLastName: form.firstLastName.trim(),
            secondLastName: form.secondLastName.trim(),
            userRole: form.userRole
        };

        if (props.isEdit) {
            await updateUser(form.userId, payload);
            onSuccess('Usuario actualizado correctamente.');
        } else {
            await createUser(payload);
            onSuccess('Usuario creado correctamente.');
            clearForm();
        }
    }, props.isEdit ? 'No se pudo actualizar el usuario.' : 'No se pudo crear el usuario.');
}

function clearForm() {
    form.nationalId = '';
    form.businessEmail = '';
    form.firstName = '';
    form.firstLastName = '';
    form.secondLastName = '';
    form.userRole = '';
}

function onSuccessClose() {
    router.push({ path: '/admin/users' });
}
</script>

<template>
    <form class="relative z-10 w-full max-w-180 rounded-md bg-white px-9 py-9 shadow-md" @submit.prevent="submit">
        <div class="grid grid-cols-1 gap-x-6 gap-y-5 md:grid-cols-2">
            <div class="md:col-span-2">
                <AppInput v-model="form.nationalId" label="Cédula" maxlength="9" placeholder="Ej. 123456789" :error="errors.fields.nationalId" :disabled="props.isEdit" />
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
            <label class="mb-2 block text-sm font-medium text-content">rol</label>
            <select
                v-model="form.userRole"
                class="h-10 w-full rounded-md border border-secondary bg-body px-4 text-[15px] text-content outline-none transition hover:cursor-pointer focus:border-primary"
                :class="{ 'border-error text-error': errors.fields.userRole }"
            >
                <option value="" disabled>Asignar Rol de Acceso</option>
                <option value="Administrator">Administrador</option>
                <option value="Operator">Operador</option>
            </select>
            <p v-if="errors.fields.userRole" class="mt-1 text-sm text-error">{{ errors.fields.userRole }}</p>
        </div>

        <div class="mt-7 border-t border-secondary pt-9">
            <div class="flex justify-center gap-4">
                <AppButton :loading="isLoading" type="submit" variant="primary">
                    {{ props.isEdit ? 'Guardar cambios' : 'Crear Usuario' }}
                </AppButton>
            </div>
        </div>

        <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="errors.fields" />
        <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    </form>
</template>