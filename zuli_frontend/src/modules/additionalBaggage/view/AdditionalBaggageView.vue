<script setup>
import { ref, watch } from 'vue'
import { onBeforeRouteLeave, useRouter } from 'vue-router'
import AppButton from '../../../shared/AppButton.vue'
import ErrorModal from '../../../shared/ErrorModal.vue'
import { useReservationSearchStore } from '../../landing/store/reservationSearchStore'
import { useAdditionalBaggage } from '../composable/useAdditionalBaggage'

const router = useRouter()
const reservationSearchStore = useReservationSearchStore()
const showErrorModal = ref(false)

const {
  maxCarryOn,
  maxCheckedBags,
  isLoading,
  isPaying,
  error,
  success,
  paymentConfirmation,
  hasChanges,
  passengers,
  selectedItems,
  totalAmount,
  trip,
  getPassengerTotal,
  updateBaggage,
  payAdditionalBaggage,
} = useAdditionalBaggage()

function formatMoney(value) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value ?? 0)
}

function goBack() {
  router.back()
}

watch(error, (message) => {
  if (message) showErrorModal.value = true
})

onBeforeRouteLeave((to) => {
  if (to.name !== 'consult') {
    reservationSearchStore.clearSearch()
  }
})
</script>

<template>
  <main class="flex-1 bg-[#F7F3F2] px-4 py-8 text-font sm:px-6 lg:px-8 lg:py-12">
    <div class="mx-auto mb-6 w-full max-w-7xl">
      <button
        type="button"
        class="flex items-center gap-2 text-lg font-semibold text-[#8B0D16] transition hover:opacity-80"
        @click="goBack"
      >
        <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0 7-7m-7 7h18" />
        </svg>
        Volver a mis vuelos
      </button>
    </div>

    <section v-if="paymentConfirmation" class="mx-auto w-full max-w-7xl space-y-6">
      <div class="rounded-[18px] bg-white p-5 shadow-md sm:p-8">
        <div class="flex flex-col gap-6 lg:flex-row lg:items-center lg:justify-between">
          <div class="flex flex-col gap-5 sm:flex-row sm:items-start sm:gap-6">
            <div class="flex h-16 w-16 shrink-0 items-center justify-center rounded-full bg-[#F4EAEA] text-[#8B0D16] sm:h-24 sm:w-24">
              <svg class="h-10 w-10 sm:h-14 sm:w-14" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2 9.8 4.2 6.7 3.7 5.7 6.7 2.8 8.2 4.2 11 2.8 13.8 5.7 15.3 6.7 18.3 9.8 17.8 12 20 14.2 17.8 17.3 18.3 18.3 15.3 21.2 13.8 19.8 11 21.2 8.2 18.3 6.7 17.3 3.7 14.2 4.2 12 2Zm-1.1 13.1-3.2-3.2 1.4-1.4 1.8 1.8 4-4 1.4 1.4-5.4 5.4Z" />
              </svg>
            </div>

            <div class="min-w-0">
              <h1 class="text-2xl font-black leading-tight text-[#8B0D16] sm:text-4xl">Compra de equipaje completada.</h1>
              <p class="mt-3 max-w-2xl text-base text-font/75 sm:text-lg">
                Tus maletas adicionales fueron agregadas correctamente a la reserva.
              </p>
              <div class="mt-6 flex w-full flex-col gap-2 rounded-lg border border-[#E5D6D6] bg-[#FAF5F5] px-4 py-4 text-sm uppercase tracking-[0.12em] text-font/70 sm:inline-flex sm:w-auto sm:flex-row sm:px-5 sm:text-lg sm:tracking-[0.16em]">
                <span>Código de reserva:</span>
                <span class="break-all font-bold tracking-[0.16em] text-[#8B0D16] sm:ml-3 sm:break-normal sm:tracking-[0.2em]">{{ paymentConfirmation.reservationCode }}</span>
              </div>
            </div>
          </div>

          <div class="border-t border-border-soft pt-6 lg:min-w-[360px] lg:border-l lg:border-t-0 lg:pl-8 lg:pt-0">
            <div class="flex items-center gap-3 text-base text-font sm:text-lg">
              <span class="flex h-6 w-6 items-center justify-center rounded-full bg-[#9C7714] text-white">✓</span>
              Pago de equipaje registrado
            </div>
            <div class="mt-4 flex items-center gap-3 text-base text-font sm:text-lg">
              <span class="flex h-6 w-6 items-center justify-center rounded-full bg-[#9C7714] text-white">✓</span>
              Total de reserva actualizado
            </div>
          </div>
        </div>
      </div>

      <div class="grid gap-6 lg:grid-cols-[360px_1fr]">
        <aside class="rounded-[18px] bg-white p-7 shadow-md">
          <h2 class="text-xl font-semibold text-font">Pago y Resumen</h2>

          <div class="mt-7 rounded-xl bg-[#F3EEEE] p-5">
            <p class="text-sm text-font/65">Servicio comprado</p>
            <p class="mt-1 text-lg font-bold uppercase text-font">Equipaje adicional</p>
          </div>

          <div class="mt-7">
            <h3 class="text-sm font-bold uppercase tracking-wide text-font">Desglose de equipaje</h3>
            <div class="mt-4 space-y-4">
              <div
                v-for="item in paymentConfirmation.items"
                :key="`summary-${item.passengerId}-${item.label}`"
                class="rounded-xl bg-[#FBF8F8] p-4"
              >
                <p class="text-sm font-bold text-font">{{ item.passengerName }}</p>
                <div class="mt-2 flex justify-between gap-4 text-sm text-font/70">
                  <span>{{ item.label }}</span>
                  <strong class="shrink-0 text-font">{{ formatMoney(item.price) }}</strong>
                </div>
              </div>
            </div>
          </div>

          <div class="mt-7 border-t border-border-soft pt-6">
            <div class="flex justify-between text-font/75">
              <span>Total equipaje</span>
              <strong class="text-[#8B0D16]">{{ formatMoney(paymentConfirmation.paidAmount) }}</strong>
            </div>
          </div>
        </aside>

        <section class="space-y-6">
          <div class="rounded-[18px] bg-white p-7 shadow-md">
            <h2 class="flex items-center gap-3 text-xl font-semibold text-font">
              <svg class="h-6 w-6 text-[#8B0D16]" viewBox="0 0 24 24" fill="currentColor">
                <path d="M2 16.5 22 21v-2.3L2 12v4.5Zm1.3-8.1 5.2 1.4 2.5-2.5L4.2 4.6 2.8 6l3.5 3.5-3-.8v-.3Zm9.3 2.5 3.7-3.7c.8-.8 2.1-.8 2.9 0 .8.8.8 2.1 0 2.9l-4.6 4.6L2 11.3V9l10.6 1.9Z" />
              </svg>
              Información del Itinerario
            </h2>

            <div class="mt-9 grid gap-6 md:grid-cols-[34px_1fr] md:items-center">
              <div class="hidden h-full flex-col items-center md:flex">
                <div class="flex h-9 w-9 items-center justify-center rounded-full bg-[#8B0D16] text-white">
                  <svg class="h-5 w-5" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M21 16v-2l-8-5V3.5a1.5 1.5 0 0 0-3 0V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5L21 16Z" />
                  </svg>
                </div>
                <div class="mt-2 h-16 border-l-2 border-dashed border-[#E7D6D6]"></div>
              </div>

              <div class="grid gap-6 md:grid-cols-[1fr_240px_1fr] md:items-center">
                <div>
                  <p class="text-lg font-semibold text-font/70">Origen</p>
                  <p class="mt-1 text-xl font-medium text-font">{{ paymentConfirmation.origin }} - {{ paymentConfirmation.originName }}</p>
                  <p class="mt-2 text-base text-font/65">{{ paymentConfirmation.departureDateText }}</p>
                </div>

                <div class="text-center">
                  <div class="flex items-center gap-3">
                    <div class="h-px flex-1 bg-[#E7D6D6]"></div>
                    <p class="text-lg font-semibold text-[#8B0D16]">{{ paymentConfirmation.flightNumber }}</p>
                    <div class="h-px flex-1 bg-[#E7D6D6]"></div>
                  </div>
                  <p class="mt-3 text-lg font-medium text-font">{{ paymentConfirmation.airlineName }} - {{ paymentConfirmation.durationText }}</p>
                </div>

                <div class="text-left md:text-right">
                  <p class="text-lg font-semibold text-font/70">Destino</p>
                  <p class="mt-1 text-xl font-medium text-font">{{ paymentConfirmation.destination }} - {{ paymentConfirmation.destinationName }}</p>
                  <p class="mt-2 text-base text-font/65">{{ paymentConfirmation.arrivalDateText }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="rounded-[18px] bg-white p-7 shadow-md">
            <h2 class="text-xl font-semibold text-font">Equipaje comprado</h2>
            <div class="mt-6 overflow-hidden rounded-xl border border-border-soft">
              <div class="grid grid-cols-[1fr_1fr_auto] bg-[#F0E9E9] px-5 py-4 font-bold text-font/75">
                <span>Pasajero</span>
                <span>Detalle</span>
                <span>Monto</span>
              </div>
              <div
                v-for="item in paymentConfirmation.items"
                :key="`${item.passengerId}-${item.label}`"
                class="grid grid-cols-[1fr_1fr_auto] border-t border-border-soft px-5 py-4 text-font"
              >
                <strong>{{ item.passengerName }}</strong>
                <span>{{ item.label }}</span>
                <strong>{{ formatMoney(item.price) }}</strong>
              </div>
            </div>
          </div>

          <div class="flex justify-end">
            <router-link to="/" class="inline-flex rounded-full bg-[#8B0D16] px-8 py-4 font-semibold text-white shadow-lg transition hover:opacity-90">
              Volver al inicio
            </router-link>
          </div>
        </section>
      </div>
    </section>

    <div v-else class="mx-auto grid w-full max-w-7xl gap-8 lg:grid-cols-[minmax(0,1fr)_380px] xl:gap-12">
      <section class="space-y-6">
        <div v-if="isLoading" class="border border-border-soft bg-text-box p-6 text-font shadow-sm sm:p-8">
          Cargando información de equipaje...
        </div>

        <div v-if="success" class="border border-green-300 bg-green-50 p-6 text-sm font-semibold text-green-700 shadow-sm sm:p-8">
          {{ success }}
        </div>

        <div class="border border-border-soft bg-text-box p-6 shadow-sm sm:p-8">
          <p class="text-sm font-semibold uppercase tracking-[0.22em] text-sumary/80">Gestion de Equipaje Adicional</p>
          <h1 class="mt-4 text-3xl font-black text-font sm:text-4xl">Personaliza tu equipaje</h1>
          <p class="mt-2 max-w-2xl text-sm text-font/75 sm:text-base">Agrega maletas antes de viajar y revisa el total actualizado para cada pasajero.</p>
        </div>

        <article v-for="(passenger, index) in passengers" :key="passenger.id" class="overflow-hidden border border-border-soft bg-text-box shadow-sm">
          <div class="flex flex-col gap-2 border-b border-border-soft bg-white/55 px-5 py-5 sm:flex-row sm:items-center sm:justify-between sm:px-7">
            <div>
              <p class="text-sm font-semibold uppercase tracking-[0.16em] text-sumary/75">Pasajero {{ index + 1 }}</p>
              <h2 class="mt-1 text-2xl font-black text-font">{{ passenger.name }}</h2>
            </div>
          </div>

          <div class="space-y-4 p-5 sm:p-7">
            <div class="grid gap-4 rounded-2xl p-4 sm:grid-cols-[56px_1fr_auto] sm:items-center sm:p-5">
              <div class="flex h-12 w-12 items-center justify-center rounded-full bg-white text-sumary shadow-sm">
                <svg class="h-7 w-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M7 7h10v13H7z" />
                  <path d="M9 7V5a3 3 0 0 1 6 0v2" />
                  <path d="M10 11h4" />
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-bold text-font">Equipaje de Mano</h3>
                <p class="text-sm text-font/65">Maximo 10 kg por pieza</p>
                <p class="mt-1 text-sm font-semibold text-sumary">{{ passenger.carryOnPrice === 0 ? 'Incluido' : `+ ${formatMoney(passenger.carryOnPrice)}` }}</p>
              </div>
              <div class="flex items-center justify-between gap-3 sm:justify-end">
                <button type="button" class="h-10 w-10 border border-border-soft bg-white text-xl font-bold text-font transition hover:bg-[#F3DADA] disabled:cursor-not-allowed disabled:opacity-40" :disabled="passenger.carryOn === passenger.originalCarryOn" @click="updateBaggage(passenger.id, 'carryOn', -1)">−</button>
                <span class="w-8 text-center text-lg font-bold">{{ passenger.carryOn }}</span>
                <button type="button" class="h-10 w-10 border border-border-soft bg-white text-xl font-bold text-font transition hover:bg-[#F3DADA] disabled:cursor-not-allowed disabled:opacity-40" :disabled="passenger.carryOn === maxCarryOn" @click="updateBaggage(passenger.id, 'carryOn', 1)">+</button>
              </div>
            </div>

            <div class="grid gap-4 rounded-2xl p-4 sm:grid-cols-[56px_1fr_auto] sm:items-center sm:p-5">
              <div class="flex h-12 w-12 items-center justify-center rounded-full bg-white text-sumary shadow-sm">
                <svg class="h-7 w-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="6" y="4" width="12" height="16" rx="2" />
                  <path d="M9 4V2h6v2" />
                  <path d="M9 8h6" />
                  <path d="M9 20v2" />
                  <path d="M15 20v2" />
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-bold text-font">Equipaje Facturado</h3>
                <p class="text-sm text-font/65">Maximo 23 kg por maleta</p>
                <p class="mt-1 text-sm font-semibold text-sumary">{{ passenger.checkedBags === maxCheckedBags ? 'Maximo alcanzado' : `Siguiente ${formatMoney(passenger.checkedBagPrices[passenger.checkedBags])}` }}</p>
              </div>
              <div class="flex items-center justify-between gap-3 sm:justify-end">
                <button type="button" class="h-10 w-10 border border-border-soft bg-white text-xl font-bold text-font transition hover:bg-[#F3DADA] disabled:cursor-not-allowed disabled:opacity-40" :disabled="passenger.checkedBags === passenger.originalCheckedBags" @click="updateBaggage(passenger.id, 'checkedBags', -1)">−</button>
                <span class="w-8 text-center text-lg font-bold">{{ passenger.checkedBags }}</span>
                <button type="button" class="h-10 w-10 border border-border-soft bg-white text-xl font-bold text-font transition hover:bg-[#F3DADA] disabled:cursor-not-allowed disabled:opacity-40" :disabled="passenger.checkedBags === maxCheckedBags" @click="updateBaggage(passenger.id, 'checkedBags', 1)">+</button>
              </div>
            </div>
          </div>
        </article>
      </section>

      <aside class="lg:sticky lg:top-8 lg:self-start">
        <div class="bg-sumary p-7 text-white shadow-xl sm:p-9">
          <p class="text-sm font-semibold uppercase tracking-[0.2em] text-white/60">Resumen</p>
          <h2 class="mt-2 text-3xl font-black">Resumen de Compra</h2>
          <div class="mt-7 rounded-2xl border border-white/15 p-5">
            <p class="text-2xl font-black">{{ trip.origin }} → {{ trip.destination }}</p>
            <p class="mt-1 text-lg font-semibold text-white/80">Equipaje Adicional</p>
            <p class="mt-1 text-sm text-white/55">{{ trip.routeLabel }}</p>
          </div>
          <div class="mt-7 space-y-5">
            <div v-for="passenger in passengers" :key="`summary-${passenger.id}`">
              <p class="font-bold">{{ passenger.name }}</p>
              <div class="mt-2 space-y-1 text-sm text-white/80">
                <div v-if="selectedItems.filter((selectedItem) => selectedItem.passengerId === passenger.id).length === 0" class="text-white/55">Sin equipaje adicional agregado</div>
                <div v-for="item in selectedItems.filter((selectedItem) => selectedItem.passengerId === passenger.id)" :key="`${passenger.id}-${item.label}`" class="flex justify-between gap-4">
                  <span>{{ item.label }}</span>
                  <span>{{ formatMoney(item.price) }}</span>
                </div>
              </div>
            </div>
          </div>
          <div class="mt-8 border-t border-white/20 pt-6">
            <div class="flex items-end justify-between gap-4"><span class="text-lg text-white/75">Total a pagar</span></div>
            <div class="flex items-end justify-between gap-4"><span class="text-4xl font-black">{{ formatMoney(totalAmount) }}</span></div>
            <AppButton variant="primary" size="lg" class="!mt-8 !w-full !rounded-none !bg-gold !text-xl !font-black !text-black hover:!brightness-105" :disabled="isPaying || !hasChanges" @click="payAdditionalBaggage">
              {{ isPaying ? 'Procesando...' : 'Pagar' }}
            </AppButton>
          </div>
        </div>
      </aside>
    </div>

    <ErrorModal v-model="showErrorModal" title="No se pudo agregar equipaje" :message="error || 'No se pudo agregar el equipaje adicional.'" />
  </main>
</template>
