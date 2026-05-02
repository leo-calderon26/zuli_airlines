<script setup>
import { reactive } from 'vue';

const emit = defineEmits(['submit']);

const form = reactive({
    model: '',
    weight: 0,
    numberEconomyClassRows: 0,
    numberSeatingRowsEconomy: 0,
    numberFirstClassRows: 0,
    numberSeatingRowsFirst: 0,
});

const fields = [
    { key: 'model', label: 'Modelo', type: 'text', min: undefined, step: undefined },
    { key: 'weight', label: 'Peso (kg)', type: 'number', min: 1, step: 1 },
    { key: 'numberEconomyClassRows', label: 'Filas clase economica', type: 'number', min: 0, step: 1 },
    { key: 'numberSeatingRowsEconomy', label: 'Asientos por fila economica', type: 'number', min: 0, step: 1 },
    { key: 'numberFirstClassRows', label: 'Filas primera clase', type: 'number', min: 0, step: 1 },
    { key: 'numberSeatingRowsFirst', label: 'Asientos por fila primera clase', type: 'number', min: 0, step: 1 },
];

const handleSubmit = () => {
    emit('submit', { ...form });
};
</script>

<template>
    <form class="form-card" @submit.prevent="handleSubmit">
        <div class="form-grid">
            <div v-for="field in fields" :key="field.key" class="form-field group">
                <input
                    :id="field.key"
                    v-model="form[field.key]"
                    :name="field.key"
                    :type="field.type"
                    :min="field.min"
                    :step="field.step"
                    class="form-input peer"
                    placeholder=" "
                    required
                />
                <label :for="field.key" class="form-label">
                    {{ field.label }}
                </label>
            </div>
        </div>

        <button type="submit" class="submit-btn">
            Guardar aeronave
        </button>
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