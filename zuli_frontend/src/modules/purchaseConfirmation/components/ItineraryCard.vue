<template>
  <InfoCard title="Información del itinerario">
    <div
      v-if="flights.length === 0"
      class="text-[#554241]"
    >
      No hay vuelos asociados a esta reserva.
    </div>

    <div
      v-else
      class="space-y-5"
    >
      <template
        v-for="(flight, index) in flights"
        :key="flight.flightId"
      >
        <div class="relative border-l-2 border-dashed border-[#dcc0be]/60 pl-8">
          <span class="absolute -left-[10px] top-0 flex h-5 w-5 items-center justify-center rounded-full bg-primary">
            <span class="h-2 w-2 rounded-full bg-white"></span>
          </span>

          <div class="grid gap-4 md:grid-cols-[1fr_auto_1fr] md:items-center">
            <div>
              <p class="text-sm text-[#554241]">Origen</p>

              <p class="font-medium text-content">
                {{ flight.originAirportCode }} - {{ flight.originAirportName }}
              </p>

              <p class="text-sm text-[#554241]">
                {{ formatDateTime(flight.departureDateTime) }}
              </p>
            </div>

            <div class="hidden min-w-40 text-center md:block">
              <div class="flex items-center gap-2">
                <span class="h-px flex-1 bg-[#dcc0be]/60"></span>

                <span class="text-sm font-semibold text-primary">
                  Vuelo {{ flight.flightNumber }}
                </span>

                <span class="h-px flex-1 bg-[#dcc0be]/60"></span>
              </div>

              <p class="mt-2 text-sm text-content">
                {{ flight.airlineName }}
              </p>
            </div>

            <div class="md:text-right">
              <p class="text-sm text-[#554241]">Destino</p>

              <p class="font-medium text-content">
                {{ flight.destinationAirportCode }} - {{ flight.destinationAirportName }}
              </p>

              <p class="text-sm text-[#554241]">
                {{ formatDateTime(flight.arrivalDateTime) }}
              </p>
            </div>
          </div>
        </div>

        <div
          v-if="connectionText(index)"
          class="flex justify-center"
        >
          <div class="rounded-lg bg-[#F4F3F3] px-5 py-2 text-sm font-semibold text-content">
            {{ connectionText(index) }}
          </div>
        </div>
      </template>
    </div>
  </InfoCard>
</template>

<script setup>
import InfoCard from './InfoCard.vue'

const props = defineProps({
  flights: {
    type: Array,
    default: () => [],
  },
})

function formatDateTime(value) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('es-CR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

function connectionText(index) {
  const currentFlight = props.flights[index]
  const nextFlight = props.flights[index + 1]

  if (!currentFlight || !nextFlight) return ''

  const currentArrival = new Date(currentFlight.arrivalDateTime)
  const nextDeparture = new Date(nextFlight.departureDateTime)
  const minutes = Math.round((nextDeparture - currentArrival) / 60000)

  if (Number.isNaN(minutes) || minutes <= 0) return ''

  const hours = Math.floor(minutes / 60)
  const remainingMinutes = minutes % 60

  if (hours > 0 && remainingMinutes > 0) {
    return `Escala de ${hours}h ${remainingMinutes}min`
  }

  if (hours > 0) {
    return `Escala de ${hours}h`
  }

  return `Escala de ${remainingMinutes}min`
}
</script>