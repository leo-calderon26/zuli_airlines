<template>
  <section class="rounded-[18px] bg-white px-7 py-7 shadow-md">
    <div class="mb-8 flex items-center gap-3">
      <svg
        class="h-[19px] w-[21px] text-primary"
        viewBox="0 0 21 19"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M2 18.15V16.15H20V18.15H2ZM3.75 13.15L0 6.9L2.4 6.25L5.2 8.6L8.7 7.675L3.525 0.775L6.425 0L13.9 6.275L18.15 5.125C18.6833 4.975 19.1875 5.0375 19.6625 5.3125C20.1375 5.5875 20.45 5.99167 20.6 6.525C20.75 7.05833 20.6875 7.5625 20.4125 8.0375C20.1375 8.5125 19.7333 8.825 19.2 8.975L3.75 13.15Z"
          fill="currentColor"
        />
      </svg>

      <h2 class="text-[18px] font-medium text-content">
        Información del Itinerario
      </h2>
    </div>

    <div class="space-y-8">
      <template
        v-for="(flight, index) in flights"
        :key="flight.flightId"
      >
        <div class="relative border-l-2 border-dashed border-[#eadfdf] pl-10">
          <span class="absolute -left-[14px] top-1 flex h-7 w-7 items-center justify-center rounded-full bg-primary text-white">
            <svg
              class="h-4 w-4"
              viewBox="0 0 24 24"
              fill="currentColor"
            >
              <path d="M21 16v-2l-8-5V3.5a1.5 1.5 0 0 0-3 0V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5Z" />
            </svg>
          </span>

          <div class="grid gap-4 md:grid-cols-[1fr_auto_1fr] md:items-start">
            <div>
              <p class="text-[18px] text-[#554241]">
                Origen
              </p>

              <p class="text-[18px] text-content">
                {{ flight.originAirportCode }} - {{ flight.originAirportName }}
              </p>

              <p class="mt-1 text-[16px] text-[#554241]">
                {{ formatDateTime(flight.departureDateTime) }}
              </p>
            </div>

            <div class="min-w-[240px] text-center">
              <div class="flex items-center gap-3">
                <span class="h-px flex-1 bg-[#eadfdf]"></span>

                <span class="text-[18px] font-medium text-primary">
                  {{ flight.flightNumber }}
                </span>

                <span class="h-px flex-1 bg-[#eadfdf]"></span>
              </div>

              <p class="mt-1 text-[18px] text-content">
                {{ flight.airlineName }} - {{ flight.duration || durationText(flight) }}
              </p>
            </div>

            <div class="text-right">
              <p class="text-[18px] text-[#554241]">
                Destino
              </p>

              <p class="text-[18px] text-content">
                {{ flight.destinationAirportCode }} - {{ flight.destinationAirportName }}
              </p>

              <p class="mt-1 text-[16px] text-[#554241]">
                {{ formatDateTime(flight.arrivalDateTime) }}
              </p>
            </div>
          </div>
        </div>

        <div
          v-if="connectionText(index)"
          class="flex justify-center"
        >
          <div class="flex items-center gap-3 rounded-[12px] bg-[#f2efef] px-6 py-3">
            <svg
              class="h-5 w-5 text-[#92701B]"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <circle cx="12" cy="12" r="8" />
              <path d="M12 8v4l2.5 2.5" />
            </svg>

            <span class="text-[16px] font-semibold text-content">
              {{ connectionText(index) }}
            </span>
          </div>
        </div>
      </template>
    </div>
  </section>
</template>

<script setup>
const props = defineProps({
  flights: {
    type: Array,
    default: () => [],
  },
})

function formatDateTime(value) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('en-GB', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value)).replace(',', '')
}

function durationText(flight) {
  const start = new Date(flight.departureDateTime)
  const end = new Date(flight.arrivalDateTime)
  const minutes = Math.round((end - start) / 60000)

  if (Number.isNaN(minutes) || minutes <= 0) return ''

  const hours = Math.floor(minutes / 60)
  const remainingMinutes = minutes % 60

  if (hours > 0 && remainingMinutes > 0) {
    return `${hours}h ${remainingMinutes}m`
  }

  if (hours > 0) {
    return `${hours}h`
  }

  return `${remainingMinutes}m`
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
  const city = nextFlight.originAirportName
  const code = nextFlight.originAirportCode

  if (hours > 0 && remainingMinutes > 0) {
    return `Escala en ${city} (${code}): ${hours}h ${remainingMinutes}m`
  }

  if (hours > 0) {
    return `Escala en ${city} (${code}): ${hours}h`
  }

  return `Escala en ${city} (${code}): ${remainingMinutes}m`
}
</script>