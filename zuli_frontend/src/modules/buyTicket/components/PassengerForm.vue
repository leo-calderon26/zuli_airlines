<script setup>
import AppInput from '../../../shared/AppInput.vue'
import AppButton from '../../../shared/AppButton.vue'

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
                <h3 class="text-2xl font-bold text-font flex items-center gap-2"><img src="../assets/user.png" alt="" class="h-6 w-6 icon-font" />Pasajero {{ index + 1 }}</h3>
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
                <AppInput
                    :model-value="passenger.passportCountry"
                    @update:model-value="updatePassenger(index, 'passportCountry', $event)"
                    label="País del Pasaporte"
                    placeholder="Ej: Costa Rica"
                    :error="errors?.[index]?.passportCountry"
                    required
                />
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
            variant="outline"
            class="!w-full !border-dashed !py-4"
            @click="$emit('add-passenger')"
        >+ Añadir pasajero</AppButton>

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
