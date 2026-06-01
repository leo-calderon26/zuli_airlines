<template>
  <InfoCard title="Información de pasajeros">
    <div
      v-if="passengers.length === 0"
      class="text-[#554241]"
    >
      No hay pasajeros asociados a esta reserva.
    </div>

    <div
      v-else
      class="overflow-x-auto"
    >
      <table class="w-full min-w-[680px] border-collapse">
        <thead>
          <tr class="border-b border-[#dcc0be]/30 bg-[#F4F3F3] text-left">
            <th class="px-4 py-3 font-semibold text-[#554241]">Pasajero</th>
            <th class="px-4 py-3 font-semibold text-[#554241]">Nacimiento</th>
            <th class="px-4 py-3 font-semibold text-[#554241]">Pasaporte</th>
            <th class="px-4 py-3 font-semibold text-[#554241]">Equipaje</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="passenger in passengers"
            :key="`${passenger.fullName}-${passenger.birthDate}`"
            class="border-b border-[#dcc0be]/20"
          >
            <td class="px-4 py-3">
              <p class="font-semibold text-content">
                {{ passenger.fullName }}
              </p>

              <p class="text-xs text-[#554241]">
                {{ passengerType(passenger.birthDate) }}
              </p>
            </td>

            <td class="px-4 py-3">
              <p class="text-content">
                {{ formatDate(passenger.birthDate) }}
              </p>

              <p class="text-xs text-[#554241]">
                {{ passenger.gender }}
              </p>
            </td>

            <td class="px-4 py-3">
              <p class="text-content">
                {{ passenger.passportCountry }}
              </p>

              <p class="text-xs text-[#554241]">
                Pasaporte
              </p>
            </td>

            <td class="px-4 py-3">
              <p class="font-semibold text-content">
                {{ passenger.checkedBaggageQuantity }} maleta(s)
              </p>

              <p class="text-xs text-[#554241]">
                {{ passenger.carryOnQuantity }} carry on
              </p>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </InfoCard>
</template>

<script setup>
import InfoCard from './InfoCard.vue'

defineProps({
  passengers: {
    type: Array,
    default: () => [],
  },
})

function formatDate(value) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('es-CR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  }).format(new Date(value))
}

function passengerType(birthDate) {
  if (!birthDate) return 'Pasajero'

  const birth = new Date(birthDate)
  const today = new Date()
  let age = today.getFullYear() - birth.getFullYear()
  const monthDifference = today.getMonth() - birth.getMonth()

  if (
    monthDifference < 0 ||
    (monthDifference === 0 && today.getDate() < birth.getDate())
  ) {
    age -= 1
  }

  return age >= 18 ? 'Adulto' : 'Menor'
}
</script>