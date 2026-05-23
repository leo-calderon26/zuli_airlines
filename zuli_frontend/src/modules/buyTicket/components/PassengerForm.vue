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
    buyerName: { type: String, default: '' },
    email: { type: String, default: '' },
    phone: { type: String, default: '' },
    flightClass: { type: String, default: 'Turista' },
    paymentMethod: { type: String, default: 'card' },
    carryOnPrice: { type: Number, default: 0 },
    checkedPrice: { type: Number, default: 0 },
    carryOnWeight: { type: Number, default: 0 },
    checkedMaxWeight: { type: Number, default: 0 },
    errors: { type: Array, default: () => [] },
    contactErrors: { type: Object, default: () => ({}) }
});

const emit = defineEmits([
    'update:passengers',
    'update:buyerName',
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
                    label="Nombre(s)"
                    placeholder="Nombres"
                    :error="errors?.[index]?.firstName"
                    required
                />
                <AppInput
                    :model-value="passenger.lastName"
                    @update:model-value="updatePassenger(index, 'lastName', $event)"
                    label="Apellido(s)"
                    placeholder="Apellidos"
                    :error="errors?.[index]?.lastName"
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
                    :model-value="passenger.birthDate"
                    @update:model-value="updatePassenger(index, 'birthDate', $event)"
                    label="Fecha de Nacimiento"
                    type="date"
                    :error="errors?.[index]?.birthDate"
                    required
                />
            </div>

            <div class="border-t border-border-soft mt-6 pt-6">
                <h4 class="text-2xl font-bold text-font mb-4 flex items-center gap-2"><img src="../assets/luggage.png" alt="" class="h-6 w-6 icon-font" />Equipaje</h4>
                <div class="flex justify-between items-center">
                    <div>
                        <span class="text-sm text-font">Maleta documentada</span>
                        <p v-if="checkedPrice" class="text-xs text-sumary mt-0.5">${{ checkedPrice }} c/u <span v-if="checkedMaxWeight">(hasta {{ checkedMaxWeight }} kg)</span></p>
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
                            @click="updatePassenger(index, 'checkedBaggage', (passenger.checkedBaggage || 0) + 1)"
                            class="w-8 h-8 flex items-center justify-center border border-border-soft bg-white text-sm font-bold hover:bg-[#F3DADA] transition"
                        >+</button>
                    </div>
                </div>
                <div class="flex justify-between items-center mt-4 pt-4 border-t border-border-soft/50">
                    <div>
                        <span class="text-sm text-font">Equipaje de mano</span>
                        <p v-if="carryOnPrice" class="text-xs text-sumary mt-0.5">${{ carryOnPrice }} c/u <span v-if="carryOnWeight">(hasta {{ carryOnWeight }} kg)</span></p>
                    </div>
                    <label class="flex items-center gap-2 cursor-pointer">
                        <input
                            type="checkbox"
                            :checked="passenger.carryOn === 1"
                            @change="updatePassenger(index, 'carryOn', $event.target.checked ? 1 : 0)"
                            class="w-4 h-4 accent-sumary"
                        />
                        <span class="text-sm font-semibold">{{ passenger.carryOn ? '1 equipaje' : 'Sin equipaje' }}</span>
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
                    :model-value="buyerName"
                    @update:model-value="$emit('update:buyerName', $event)"
                    label="Nombre Completo"
                    placeholder="Nombre del comprador"
                    :error="contactErrors?.buyerName"
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
