<script setup>
import { reactive, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAirport } from '../composable/useAirport';
import { getCountries, getCitiesByCountry } from '../service/locationService';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import { useForm } from '../../../shared/useForm.js';
import authService from "../../auth/services/authService";

const props = defineProps({
    airport: {
        type: Object,
        default: null
    },
    isEdit: {
        type: Boolean,
        default: false
    }
});

const router = useRouter();
const { addAirport, updateAirport } = useAirport();
const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess, handleSubmit } = useForm();

const countries = ref([]);
const cities = ref([]);
const selectedCountryId = ref('');
const isCitiesLoading = ref(false);

const form = reactive({
    airportCode: '',
    name: '',
    country: '',
    city: ''
});

function syncForm(airport) {
    if (!airport) {
        return;
    }

    form.airportCode = airport.airportCode ?? '';
    form.name = airport.name ?? '';
    form.country = airport.country ?? '';
    form.city = airport.city ?? '';
}

watch(
    () => props.airport,
    (airport) => {
        syncForm(airport);
    },
    { immediate: true, deep: true }
);

function validate() {
    clearErrors();

    if (!form.airportCode || form.airportCode.trim().length < 3) {
        errors.fields.airportCode = 'El código debe tener al menos 3 caracteres';
    }
    if (!form.name.trim()) {
        errors.fields.name = 'El nombre es obligatorio';
    }
    if (!form.country.trim()) {
        errors.fields.country = 'Debe seleccionar un país';
    }
    if (!form.city.trim()) {
        errors.fields.city = 'Debe seleccionar una ciudad';
    }

    return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function submit() {
    if (!validate()) return;

    await handleSubmit(async () => {
        const data = await authService.me();

        const airportPayload = {
            airportCode: form.airportCode.toUpperCase().trim(),
            name: form.name.trim(),
            country: form.country.trim(),
            city: form.city.trim(),
            businessId: data.businessId,
        };

        if (props.isEdit) {
            await updateAirport(props.airport?.airportCode?.toUpperCase().trim() ?? form.airportCode.toUpperCase().trim(), airportPayload);
            onSuccess('El aeropuerto se ha actualizado correctamente');
        } else {
            await addAirport(airportPayload);
            onSuccess('El aeropuerto se ha creado correctamente');
        }
    }, props.isEdit ? 'Error al actualizar el aeropuerto' : 'Error al crear el aeropuerto');

    if (errors.fields.airportCode) {
        form.airportCode = '';
    }
}

function onSuccessClose() {
    router.push({ path: '/admin/airports' });
}
</script>

<template>
    <form class="form-card" @submit.prevent="submit">
        <div class="form-grid">
            <AppInput v-model="form.airportCode" label="Código del Aeropuerto (ej. SJO)" :error="errors.fields.airportCode" maxlength="10" :disabled="props.isEdit" />

            <AppInput v-model="form.name" label="Nombre del Aeropuerto" :error="errors.fields.name" />

            <div class="flex flex-col relative">
                <label class="mb-2 block text-sm font-medium text-content" :class="{ '!text-error': errors.fields?.country }">
                    País <span class="text-error">*</span>
                </label>
                
                <Listbox v-model="selectedCountryId" @update:modelValue="onCountryChange">
                    <div class="relative">
                        <ListboxButton 
                            class="relative w-full cursor-default rounded-base border bg-body px-3 py-2.5 text-left text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
                            :class="{ 'border-error text-error': errors.fields?.country }"
                        >
                            <span class="block truncate">{{ form.country || 'Seleccione un país' }}</span>
                            <span class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2">
                                <ChevronDownIcon class="size-5 text-gray-400" aria-hidden="true" />
                            </span>
                        </ListboxButton>

                        <transition leave-active-class="transition duration-100 ease-in" leave-from-class="opacity-100" leave-to-class="opacity-0">
                            <ListboxOptions class="absolute z-20 mt-1 max-h-[10.0rem] w-full overflow-auto rounded-md bg-white py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm">
                                <ListboxOption 
                                    v-for="country in countries" 
                                    :key="country.id" 
                                    :value="country.id" 
                                    v-slot="{ active, selected }" 
                                    as="template"
                                >
                                    <li :class="[active ? 'bg-primary/10 text-primary' : 'text-gray-900', 'relative cursor-default select-none py-2 pl-10 pr-4']">
                                        <span :class="[selected ? 'font-medium' : 'font-normal', 'block truncate']">
                                            {{ country.countryName }}
                                        </span>
                                        <span v-if="selected" class="absolute inset-y-0 left-0 flex items-center pl-3 text-primary">
                                            <CheckIcon class="size-5" aria-hidden="true" />
                                        </span>
                                    </li>
                                </ListboxOption>
                            </ListboxOptions>
                        </transition>
                    </div>
                </Listbox>
                <p v-if="errors.fields?.country" class="mt-1 text-sm text-error">{{ errors.fields.country }}</p>
            </div>

            <div class="flex flex-col relative">
                <label class="mb-2 block text-sm font-medium text-content" :class="{ '!text-error': errors.fields?.city }">
                    Ciudad <span class="text-error">*</span>
                </label>

                <Listbox v-model="form.city" :disabled="!selectedCountryId || isCitiesLoading">
                    <div class="relative">
                        <ListboxButton 
                            class="relative w-full cursor-default rounded-base border bg-body px-3 py-2.5 text-left text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed"
                            :class="{ 'border-error text-error': errors.fields?.city }"
                        >
                            <span class="block truncate">
                                {{ form.city || (isCitiesLoading ? 'Cargando ciudades...' : 'Seleccione una ciudad') }}
                            </span>
                            <span class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2">
                                <ChevronDownIcon class="size-5 text-gray-400" aria-hidden="true" />
                            </span>
                        </ListboxButton>

                        <transition leave-active-class="transition duration-100 ease-in" leave-from-class="opacity-100" leave-to-class="opacity-0">
                            <ListboxOptions class="absolute z-10 mt-1 max-h-[10.0rem] w-full overflow-auto rounded-md bg-white py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm">
                                <ListboxOption 
                                    v-for="city in cities" 
                                    :key="city.id" 
                                    :value="city.cityName" 
                                    v-slot="{ active, selected }" 
                                    as="template"
                                >
                                    <li :class="[active ? 'bg-primary/10 text-primary' : 'text-gray-900', 'relative cursor-default select-none py-2 pl-10 pr-4']">
                                        <span :class="[selected ? 'font-medium' : 'font-normal', 'block truncate']">
                                            {{ city.cityName }}
                                        </span>
                                        <span v-if="selected" class="absolute inset-y-0 left-0 flex items-center pl-3 text-primary">
                                            <CheckIcon class="size-5" aria-hidden="true" />
                                        </span>
                                    </li>
                                </ListboxOption>
                            </ListboxOptions>
                        </transition>
                    </div>
                </Listbox>
                <p v-if="errors.fields?.city" class="mt-1 text-sm text-error">{{ errors.fields.city }}</p>
            </div>
        </div>

        <div class="mt-4">
            <AppButton type="submit" variant="primary" :loading="isLoading">{{ props.isEdit ? 'Guardar' : 'Crear' }}</AppButton>
        </div>
    </form>

    <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="errors.fields" />
</template>

<style scoped>
@reference "../../../style.css";

.form-card {
    @apply w-full rounded-lg border border-gray-200 bg-white p-8 shadow-sm;
}

.form-grid {
    @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}
</style>