<script setup>
import { reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAircraft } from '../composable/useAircraft';

const router = useRouter();
const { addAircraft } = useAircraft();

// ID del administrador
const ADMIN_ID = 'F25DF80C-B7D9-4A4C-88FB-90BA8D488541';

const form = reactive({
    model: '',
    weight: null,
    numberEconomyClassRows: 0,
    numberSeatingRowsEconomy: 0,
    numberFirstClassRows: 0,
    numberSeatingRowsFirst: 0,
});

const errors = reactive({
    global: '',
    fields: {},
});

const modelPattern = /^[A-Za-z0-9-]{1,15}$/;

const totalSeats = computed(() => {
    const econ = Number(form.numberEconomyClassRows) * Number(form.numberSeatingRowsEconomy);
    const first = Number(form.numberFirstClassRows) * Number(form.numberSeatingRowsFirst);
    return Number.isFinite(econ + first) ? econ + first : 0;
});

function isIntegerLike(value) {
    return Number.isInteger(Number(value)) && String(value) !== '';
}

function validate() {
    errors.global = '';
    errors.fields = {};
    if (!form.model || !modelPattern.test(form.model)) {
        errors.fields.model = 'Modelo inválido (máx 15 letras/números o guion)';
    }
    if (form.weight === null || form.weight === '' || Number(form.weight) <= 0 || isNaN(Number(form.weight))) {
        errors.fields.weight = 'Peso inválido';
    }
    const countFields = ['numberEconomyClassRows', 'numberSeatingRowsEconomy', 'numberFirstClassRows', 'numberSeatingRowsFirst'];
    for (const key of countFields) {
        const val = form[key];
        if (val === '' || val === null || isNaN(Number(val)) || Number(val) < 0 || !isIntegerLike(val)) {
            errors.fields[key] = 'Debe ingresar valores numéricos válidos mayores o iguales a cero';
        }
    }
    if (totalSeats.value >= 1000) {
        errors.fields.totalSeats = 'El número total de asientos debe ser menor a 1000';
    }
    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function handleSubmit() {
    if (!validate()) {
        if (errors.global) alert(errors.global);
        return;
    }

    const baggageCapacity = Number((Number(form.weight) * 0.3).toFixed(4));

    const aircraft = {
        model: form.model,
        capacity: totalSeats.value,
        weight: Number(form.weight),
        numberEconomyClassRows: Number(form.numberEconomyClassRows),
        numberSeatingRowsEconomy: Number(form.numberSeatingRowsEconomy),
        numberFirstClassRows: Number(form.numberFirstClassRows),
        numberSeatingRowsFirst: Number(form.numberSeatingRowsFirst),
        baggageCapacity,
        adminId: ADMIN_ID,
    };

    try {
        await addAircraft(aircraft);
        alert('La aeronave se ha creado correctamente');
        router.push({ name: 'aircraftList' });
    } catch (error) {
        errors.global = error.response?.data?.message || 'Error al crear la aeronave';
        alert(errors.global);
    }
}
</script>

<template>
    <form class="form-card" @submit.prevent="handleSubmit">
        <div class="form-grid">
            <div class="form-field group">
                <input id="model" v-model="form.model" name="model" type="text" class="form-input peer" placeholder=" " />
                <label for="model" class="form-label">Modelo</label>
                <p v-if="errors.fields.model" class="text-sm text-error">{{ errors.fields.model }}</p>
            </div>

            <div class="form-field group">
                <input id="capacity" v-model="totalSeats" name="capacity" type="number" disabled class="form-input peer" placeholder=" " />
                <label for="capacity" class="form-label">Capacidad (Calculada)</label>
            </div>

            <div class="form-field group">
                <input id="weight" v-model="form.weight" name="weight" type="number" min="1" step="1" class="form-input peer" placeholder=" " />
                <label for="weight" class="form-label">Peso soportado por la aeronave(kg)</label>
                <p v-if="errors.fields.weight" class="text-sm text-error">{{ errors.fields.weight }}</p>
            </div>

            <div class="form-field group">
                <input id="numberEconomyClassRows" v-model="form.numberEconomyClassRows" name="numberEconomyClassRows" type="number" min="0"
                 step="1" class="form-input peer" placeholder=" " />
                <label for="numberEconomyClassRows" class="form-label">Filas clase económica</label>
                <p v-if="errors.fields.numberEconomyClassRows" class="text-sm text-error">{{ errors.fields.numberEconomyClassRows }}</p>
            </div>

            <div class="form-field group">
                <input id="numberSeatingRowsEconomy" v-model="form.numberSeatingRowsEconomy" name="numberSeatingRowsEconomy" type="number" min="0" 
                step="1" class="form-input peer" placeholder=" " />
                <label for="numberSeatingRowsEconomy" class="form-label">Asientos por fila económica</label>
                <p v-if="errors.fields.numberSeatingRowsEconomy" class="text-sm text-error">{{ errors.fields.numberSeatingRowsEconomy }}</p>
            </div>

            <div class="form-field group">
                <input id="numberFirstClassRows" v-model="form.numberFirstClassRows" name="numberFirstClassRows" type="number" min="0" step="1" 
                class="form-input peer" placeholder=" " />
                <label for="numberFirstClassRows" class="form-label">Filas primera clase</label>
                <p v-if="errors.fields.numberFirstClassRows" class="text-sm text-error">{{ errors.fields.numberFirstClassRows }}</p>
            </div>

            <div class="form-field group">
                <input id="numberSeatingRowsFirst" v-model="form.numberSeatingRowsFirst" name="numberSeatingRowsFirst" type="number" min="0" step="1" 
                class="form-input peer" placeholder=" " />
                <label for="numberSeatingRowsFirst" class="form-label">Asientos por fila primera clase</label>
                <p v-if="errors.fields.numberSeatingRowsFirst" class="text-sm text-error">{{ errors.fields.numberSeatingRowsFirst }}</p>
            </div>
        </div>

        <div v-if="errors.fields.totalSeats" class="mt-2 text-p">{{ errors.fields.totalSeats }}</div>

        <div class="mt-4">
            <button type="submit" class="submit-btn">Guardar aeronave</button>
        </div>
    </form>
    
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply mx-auto w-full max-w-3xl rounded-lg border border-gray-200 bg-white p-6 shadow-sm;
}

.form-grid {
    @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}

.form-field {
    @apply relative z-0 w-full;
}

.form-input {
    @apply block w-full appearance-none border-0 border-b-2 border-gray-300 bg-transparent px-0 py-2.5 text-sm
     text-gray-900 focus:border-gold focus:outline-none focus:ring-0;
}

.form-label {
    @apply absolute top-3 -z-10 origin-[0] -translate-y-6 scale-75 transform text-sm
     text-gray-600 duration-300 peer-placeholder-shown:translate-y-0 peer-placeholder-shown:scale-100 
     peer-focus:start-0 peer-focus:-translate-y-6 peer-focus:scale-75 peer-focus:text-gold;
}

.submit-btn {
    @apply mt-2 inline-flex rounded-md border border-transparent bg-primary px-4 py-2.5 text-sm font-medium
     text-white hover:bg-select focus:outline-none focus:ring-2 focus:ring-gold;
}
</style>