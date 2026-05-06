<script setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useAirport } from '../composable/useAirport';
import authService from "../../auth/services/authService";

const router = useRouter();
const { addAirport } = useAirport();

const form = reactive({
    airportCode: '',
    name: '',
    country: '',
    city: ''
});

const errors = reactive({
    global: '',
    fields: {},
});

function validate() {
    errors.global = '';
    errors.fields = {};
    
    if (!form.airportCode || form.airportCode.trim().length < 3) {
        errors.fields.airportCode = 'El código debe tener al menos 3 caracteres';
    }
    if (!form.name.trim()) {
        errors.fields.name = 'El nombre es obligatorio';
    }
    if (!form.country.trim()) {
        errors.fields.country = 'El país es obligatorio';
    }
    if (!form.city.trim()) {
        errors.fields.city = 'La ciudad es obligatoria';
    }

    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function handleSubmit() {
    if (!validate()) {
        if (errors.global) alert(errors.global);
        return;
    }

    try {
        const data = await authService.me();

        const airportPayload = {
            airportCode: form.airportCode.toUpperCase().trim(),
            name: form.name.trim(),
            country: form.country.trim(),
            city: form.city.trim(),
            businessId: data.businessId,
        };

        await addAirport(airportPayload);
        alert('El aeropuerto se ha creado correctamente');
        
        router.push({ path: '/admin/airports' }); 
    } catch (error) {
        errors.global = error.response?.data?.message || 'Error al crear el aeropuerto';
        alert(errors.global);
    }
}
</script>

<template>
    <form class="form-card" @submit.prevent="handleSubmit">
        <div class="form-grid">
            <div class="form-field group">
                <input id="airportCode" v-model="form.airportCode" name="airportCode" type="text" class="form-input peer" placeholder=" " maxlength="10" />
                <label for="airportCode" class="form-label">Código del Aeropuerto (ej. SJO)</label>
                <p v-if="errors.fields.airportCode" class="text-sm text-error">{{ errors.fields.airportCode }}</p>
            </div>

            <div class="form-field group">
                <input id="name" v-model="form.name" name="name" type="text" class="form-input peer" placeholder=" " />
                <label for="name" class="form-label">Nombre del Aeropuerto</label>
                <p v-if="errors.fields.name" class="text-sm text-error">{{ errors.fields.name }}</p>
            </div>

            <div class="form-field group">
                <input id="country" v-model="form.country" name="country" type="text" class="form-input peer" placeholder=" " />
                <label for="country" class="form-label">País</label>
                <p v-if="errors.fields.country" class="text-sm text-error">{{ errors.fields.country }}</p>
            </div>

            <div class="form-field group">
                <input id="city" v-model="form.city" name="city" type="text" class="form-input peer" placeholder=" " />
                <label for="city" class="form-label">Ciudad</label>
                <p v-if="errors.fields.city" class="text-sm text-error">{{ errors.fields.city }}</p>
            </div>
        </div>

        <div class="mt-4">
            <button type="submit" class="submit-btn">Guardar Aeropuerto</button>
        </div>
    </form>
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply w-full max-w-none rounded-lg border border-gray-200 bg-white p-8 shadow-sm;
}

.form-grid {
    @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}

.form-field {
    @apply relative z-0 w-full;
}

.form-input {
    @apply block w-full appearance-none border-0 border-b-2 border-gray-300 bg-transparent px-0 py-3 text-base
     text-gray-900 focus:border-gold focus:outline-none focus:ring-0;
}

.form-label {
    @apply absolute top-3 -z-10 origin-[0] -translate-y-6 transform text-base
     text-gray-600 duration-300 peer-placeholder-shown:translate-y-0 peer-placeholder-shown:scale-100
     peer-focus:start-0 peer-focus:-translate-y-6 peer-focus:text-gold;
}

.submit-btn {
    @apply mt-2 inline-flex rounded-md border border-transparent bg-primary px-5 py-3 text-base font-medium
     text-white hover:bg-select focus:outline-none focus:ring-2 focus:ring-gold;
}
</style>