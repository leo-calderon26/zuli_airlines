<script setup>
import { ref, onMounted } from 'vue'
import AppInput from '../../../shared/AppInput.vue'
import AppButton from '../../../shared/AppButton.vue'
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from '@headlessui/vue'
import { ChevronDownIcon, CheckIcon } from '@heroicons/vue/20/solid'
import { getCountries } from '../../airport/service/locationService'

const paymentMethods = [
    { value: 'card', label: 'Tarjeta' },
    { value: 'paypal', label: 'PayPal' },
    { value: 'applePay', label: 'Apple Pay' },
    { value: 'googlePay', label: 'Google Pay' }
];

const props = defineProps({
    passengers: { type: Array, required: true },
    buyerFirstName: { type: String, default: '' },
    buyerFirstLastName: { type: String, default: '' },
    buyerSecondLastName: { type: String, default: '' },
    buyerBirthDate: { type: String, default: '' },
    email: { type: String, default: '' },
    phone: { type: String, default: '' },
    flightClass: { type: String, default: 'Turista' },
    paymentMethod: { type: String, default: 'card' },
    carryOnPrice: { type: Number, default: 0 },
    checkedPrice: { type: Number, default: 0 },
    maxWeightPerBag: { type: Number, default: 23 },
    maxPassengers: { type: Number, default: 1 },
    errors: { type: Array, default: () => [] },
    contactErrors: { type: Object, default: () => ({}) }
});

const emit = defineEmits([
    'update:passengers',
    'update:buyerFirstName',
    'update:buyerFirstLastName',
    'update:buyerSecondLastName',
    'update:buyerBirthDate',
    'update:email',
    'update:phone',
    'update:flightClass',
    'update:paymentMethod',
    'add-passenger',
    'remove-passenger'
]);

const countries = ref([]);

onMounted(async () => {
    try {
        countries.value = await getCountries();
    } catch (error) {
        console.error("Error al cargar la lista de países", error);
    }
});

function updatePassenger(index, field, value) {
    const updated = [...props.passengers];
    updated[index] = { ...updated[index], [field]: value };
    emit('update:passengers', updated);
}
</script>

<template>
    <div class="space-y-8">
        <div class="border border-border-soft bg-text-box p-8">
            <h3 class="text-2xl font-bold text-font mb-6 flex items-center gap-2"><img src="../assets/sit.png" alt="" class="h-6 w-6 icon-font" />Categoría de Viaje</h3>
            <div class="grid grid-cols-2 gap-6">
                <div
                    @click="$emit('update:flightClass', 'Economica')"
                    class="bg-text-box border p-6 cursor-pointer transition"
                    :class="flightClass === 'Economica' ? 'border-2 border-sumary' : 'border-border-soft hover:border-sumary'"
                >
                    <p class="text-base font-semibold mt-0.5 mb-1 opacity-70 uppercase tracking-wide">Estándar</p>
                    <p class="font-bold text-xl">Economica</p>
                    <p class="text-base mt-1 opacity-80">Cabina principal</p>
                </div>
                <div
                    @click="$emit('update:flightClass', 'Primera Clase')"
                    class="bg-text-box border p-6 cursor-pointer transition"
                    :class="flightClass === 'Primera Clase' ? 'bg-gold text-black border-2 border-gold' : 'border-border-soft hover:border-sumary'"
                >
                    <p class="text-base font-semibold mt-0.5 mb-1 opacity-70 uppercase tracking-wide">Premium</p>
                    <p class="font-bold text-xl">Primera Clase</p>
                    <p class="text-base mt-1 opacity-80">Asiento más amplio</p>
                </div>
            </div>
        </div>

        <div
            v-for="(passenger, index) in passengers"
            :key="index"
            class="border border-border-soft bg-text-box p-8"
        >
            <div class="flex items-center justify-between mb-6">
                <h3 class="text-2xl font-bold text-font flex items-center gap-2"><img src="../assets/user.png" alt="" class="h-6 w-6 icon-font" />Pasajero {{ index + 1 }} de {{ maxPassengers }}</h3>
                <AppButton
                    v-if="passengers.length > 1"
                    variant="ghost"
                    size="sm"
                    class="btn-eliminar"
                    @click="$emit('remove-passenger', index)"
                >Eliminar</AppButton>
            </div>

            <div class="grid grid-cols-2 gap-8">
                <AppInput
                    :model-value="passenger.firstName"
                    @update:model-value="updatePassenger(index, 'firstName', $event)"
                    label="Nombre"
                    placeholder="Nombre"
                    :error="errors?.[index]?.firstName"
                    required
                    class="col-span-2"
                />
                <AppInput
                    :model-value="passenger.firstLastName"
                    @update:model-value="updatePassenger(index, 'firstLastName', $event)"
                    label="Primer Apellido"
                    placeholder="Primer apellido"
                    :error="errors?.[index]?.firstLastName"
                    required
                />
                <AppInput
                    :model-value="passenger.secondLastName"
                    @update:model-value="updatePassenger(index, 'secondLastName', $event)"
                    label="Segundo Apellido"
                    placeholder="Segundo apellido"
                    :error="errors?.[index]?.secondLastName"
                    required
                />
                <AppInput
                    :model-value="passenger.birthDate"
                    @update:model-value="updatePassenger(index, 'birthDate', $event)"
                    label="Fecha de Nacimiento"
                    type="date"
                    :error="errors?.[index]?.birthDate"
                    required
                />
                <div>
                    <label class="text-sm text-font mb-2 block">Género</label>
                    <div class="flex gap-4">
                        <AppButton
                            variant="secondary"
                            size="sm"
                            @click="updatePassenger(index, 'gender', 'Male')"
                            :class="passenger.gender === 'Male' ? '!bg-font !text-white' : ''"
                        >Masculino</AppButton>
                        <AppButton
                            variant="secondary"
                            size="sm"
                            @click="updatePassenger(index, 'gender', 'Female')"
                            :class="passenger.gender === 'Female' ? '!bg-font !text-white' : ''"
                        >Femenino</AppButton>
                        <AppButton
                            variant="secondary"
                            size="sm"
                            @click="updatePassenger(index, 'gender', 'Not Disclosed')"
                            :class="passenger.gender === 'Not Disclosed' ? '!bg-font !text-white' : ''"
                        >No especifica</AppButton>
                    </div>
                    <p v-if="errors?.[index]?.gender" class="text-xs text-error mt-1">{{ errors[index].gender }}</p>
                </div>
                <div class="flex flex-col relative z-20">
                    <label class="mb-2 block text-sm font-medium text-content" :class="{ 'text-error!': errors?.[index]?.passportCountry }">
                        País del Pasaporte <span class="text-error">*</span>
                    </label>
                    
                    <Listbox 
                        :model-value="passenger.passportCountry" 
                        @update:modelValue="updatePassenger(index, 'passportCountry', $event)"
                    >
                        <div class="relative">
                            <ListboxButton 
                                class="relative w-full cursor-default rounded-base border bg-body px-3 py-2.5 text-left text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50 disabled:cursor-not-allowed"
                                :class="{ 'border-error text-error': errors?.[index]?.passportCountry }"
                            >
                                <span class="block truncate">{{ passenger.passportCountry || 'Seleccione un país' }}</span>
                                <span class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2">
                                    <ChevronDownIcon class="size-5 text-gray-400" aria-hidden="true" />
                                </span>
                            </ListboxButton>

                            <transition leave-active-class="transition duration-100 ease-in" leave-from-class="opacity-100" leave-to-class="opacity-0">
                                <ListboxOptions class="absolute z-50 mt-1 max-h-[10.0rem] w-full overflow-auto rounded-md bg-white py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-none sm:text-sm">
                                    <ListboxOption 
                                        v-for="country in countries" 
                                        :key="country.id" 
                                        :value="country.countryName" 
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
                    <p v-if="errors?.[index]?.passportCountry" class="mt-1 text-sm text-error">{{ errors[index].passportCountry }}</p>
                </div>

                <AppInput
                    :model-value="passenger.passportDueDate"
                    @update:model-value="updatePassenger(index, 'passportDueDate', $event)"
                    label="Fecha de Vencimiento del Pasaporte"
                    type="date"
                    :error="errors?.[index]?.passportDueDate"
                    required
                />
            </div>

            <div class="border-t border-border-soft mt-6 pt-6">
                <h4 class="text-2xl font-bold text-font mb-4 flex items-center gap-2"><img src="../assets/luggage.png" alt="" class="h-6 w-6 icon-font" />Equipaje</h4>
                <div class="flex justify-between items-center">
                    <div>
                        <span class="text-sm text-font">Maleta documentada</span>
                    </div>
                    <div class="flex items-center gap-3">
                        <button
                            type="button"
                            @click="updatePassenger(index, 'checkedBaggage', Math.max(0, (passenger.checkedBaggage || 0) - 1))"
                            class="w-8 h-8 flex items-center justify-center border border-border-soft bg-white text-sm font-bold hover:bg-[#F3DADA] transition"
                        >−</button>
                        <span class="w-6 text-center font-semibold">{{ passenger.checkedBaggage || 0 }}</span>
                        <button
                            type="button"
                            :disabled="(passenger.checkedBaggage || 0) >= 5"
                            @click="updatePassenger(index, 'checkedBaggage', Math.min(5, (passenger.checkedBaggage || 0) + 1))"
                            class="w-8 h-8 flex items-center justify-center border border-border-soft bg-white text-sm font-bold hover:bg-[#F3DADA] transition"
                        >+</button>
                    </div>
                </div>

                <div class="flex justify-between items-center mt-4 pt-4 border-t border-border-soft/50">
                    <div>
                        <span class="text-sm text-font">Equipaje de mano</span>
                    </div>
                    <label class="flex items-center gap-2 cursor-pointer">
                        <input
                            type="checkbox"
                            :checked="passenger.carryOn === 1"
                            @change="updatePassenger(index, 'carryOn', $event.target.checked ? 1 : 0)"
                            class="w-4 h-4 accent-sumary"
                        />
                        <span class="text-sm font-semibold">Incluir equipaje de mano</span>
                    </label>
                </div>
            </div>
        </div>

        <AppButton
            v-if="passengers.length < maxPassengers"
            variant="outline"
            class="!w-full !border-dashed !py-4"
            @click="$emit('add-passenger')"
        >+ Añadir pasajero (faltan {{ maxPassengers - passengers.length }})</AppButton>

        <div class="border border-border-soft bg-text-box p-8">
            <h3 class="text-2xl font-bold text-font mb-6 flex items-center gap-2"><img src="../assets/email.png" alt="" class="h-6 w-6 icon-font" />Información del Comprador</h3>
            <div class="grid grid-cols-2 gap-8">
                <AppInput
                    :model-value="buyerFirstName"
                    @update:model-value="$emit('update:buyerFirstName', $event)"
                    label="Nombre"
                    placeholder="Nombre"
                    :error="contactErrors?.buyerFirstName"
                    required
                />
                <AppInput
                    :model-value="buyerFirstLastName"
                    @update:model-value="$emit('update:buyerFirstLastName', $event)"
                    label="Primer Apellido"
                    placeholder="Primer apellido"
                    :error="contactErrors?.buyerFirstLastName"
                    required
                />
                <AppInput
                    :model-value="buyerSecondLastName"
                    @update:model-value="$emit('update:buyerSecondLastName', $event)"
                    label="Segundo Apellido"
                    placeholder="Segundo apellido"
                    :error="contactErrors?.buyerSecondLastName"
                    required
                />
                <AppInput
                    :model-value="buyerBirthDate"
                    @update:model-value="$emit('update:buyerBirthDate', $event)"
                    label="Fecha de Nacimiento"
                    type="date"
                    :error="contactErrors?.buyerBirthDate"
                    required
                />
                <AppInput
                    :model-value="email"
                    @update:model-value="$emit('update:email', $event)"
                    label="Correo Electrónico"
                    type="email"
                    placeholder="correo@ejemplo.com"
                    :error="contactErrors?.email"
                    required
                />
                <AppInput
                    :model-value="phone"
                    @update:model-value="$emit('update:phone', $event)"
                    label="Teléfono"
                    type="tel"
                    placeholder="+506 8888 8888"
                    :error="contactErrors?.phone"
                    required
                />
            </div>

            <div class="border-t border-border-soft mt-6 pt-6">
                <h4 class="text-sm font-semibold text-font mb-4">Método de Pago</h4>
                <div class="flex gap-4 flex-wrap">
                    <AppButton
                        v-for="method in paymentMethods"
                        :key="method.value"
                        variant="secondary"
                        size="sm"
                        @click="$emit('update:paymentMethod', method.value)"
                        :class="paymentMethod === method.value ? '!bg-font !text-white' : ''"
                    >{{ method.label }}</AppButton>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.icon-font {
  filter: brightness(0) saturate(100%) invert(15%) sepia(30%) saturate(600%) hue-rotate(340deg);
}

.btn-eliminar:hover {
  background-color: rgba(75, 30, 30, 0.1) !important;
}
</style>
