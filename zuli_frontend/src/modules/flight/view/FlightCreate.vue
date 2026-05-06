<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue'
import LandingDropdownField from '../../landing/components/LandingDropdownField.vue'
import axios from 'axios'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useAircraft } from '../../aircraft/composable/useAircraft'

const flightCreateUrl = '/api/Flight/Create'
const route = useRoute()
const { fetchAircrafts, aircrafts } = useAircraft()

const form = reactive({
  status: '',
  flightDate: '',
  realDepartureTime: '',
  realArrivalTime: '',
  checkInStartTime: '',
  checkInDeadline: '',
  touristPrice: 0,
  firstClassPrice: 0,
  duration: 0,
  airlineId: 1,
  aircraftId: '',
  itineraryId: 0,
  businessId: '',
  flightRouteId: 0,
  availableSeats: 0,
  carryOnPrice: null,
  checkedPrice: null,
  departureAirportCode: '',
  arrivalAirportCode: '',
  monday: false,
  tuesday: false,
  wednesday: false,
  thursday: false,
  friday: false,
  saturday: false,
  sunday: false,
  wifi: false,
  entertainment: false,
  food: false,
  seatSelection: false,
})

const message = ref('')
const error = ref('')

const statusOptions = [
  { label: 'Programado', value: 'Programado' },
  { label: 'Confirmado', value: 'Confirmado' },
  { label: 'Demorado', value: 'Demorado' },
  { label: 'Cancelado', value: 'Cancelado' },
]

const aircraftOptions = computed(() => {
  const options = (aircrafts.value ?? [])
    .map((aircraft, index) => ({
      label: aircraft.model ? `${aircraft.model}` : `Aeronave ${index + 1}`,
      value: aircraft.aircraftId ?? aircraft.id ?? aircraft.model ?? ''
    }))
    .filter((aircraft) => aircraft.value !== '')
  console.log('🛩️ Aircraft options computed:', options)
  return options
})

const dayLabels = [
  { key: 'monday', label: 'Lunes', bit: 1 },
  { key: 'tuesday', label: 'Martes', bit: 2 },
  { key: 'wednesday', label: 'Miercoles', bit: 4 },
  { key: 'thursday', label: 'Jueves', bit: 8 },
  { key: 'friday', label: 'Viernes', bit: 16 },
  { key: 'saturday', label: 'Sabado', bit: 32 },
  { key: 'sunday', label: 'Domingo', bit: 64 },
]

const hasSelectedRoute = computed(() => Number(form.flightRouteId) > 0)
const selectedDaysText = computed(() => {
  const selected = dayLabels
    .filter((day) => form[day.key])
    .map((day) => day.label)
  return selected.length ? selected.join(', ') : 'Sin frecuencia'
})

function formatDuration(value) {
  const totalSeconds = Number(value)
  if (!Number.isFinite(totalSeconds) || totalSeconds <= 0) return '-'
  const totalMinutes = Math.round(totalSeconds / 60)
  if (!Number.isFinite(totalMinutes) || totalMinutes <= 0) return '-'
  if (totalMinutes < 60) return `${totalMinutes} min`
  const hours = Math.floor(totalMinutes / 60)
  const minutes = totalMinutes % 60
  return `${hours} h ${minutes} min`
}

onMounted(async () => {
  console.log('🚀 FlightCreate mounted')
  await fetchAircrafts()
  console.log('✈️ Aircrafts loaded:', aircrafts.value)
})

watch(
  () => route.query,
  (query) => {
    applyRouteSelection(query)
  },
  { immediate: true }
)

function applyRouteSelection(query = {}) {
  const {
    routeId,
    departure,
    arrival,
    departureAirport,
    arrivalAirport,
    origin,
    destination,
    duration,
    estimatedDuration,
    frequency,
  } = query

  if (routeId) {
    const parsedRouteId = Number(routeId)
    form.flightRouteId = Number.isFinite(parsedRouteId) ? parsedRouteId : 0
  }

  const departureValue = departure ?? departureAirport ?? origin
  const arrivalValue = arrival ?? arrivalAirport ?? destination

  if (departureValue) {
    form.departureAirportCode = String(departureValue)
  }

  if (arrivalValue) {
    form.arrivalAirportCode = String(arrivalValue)
  }

  const durationValue = duration ?? estimatedDuration
  if (durationValue) {
    const parsedDuration = Number(durationValue)
    if (Number.isFinite(parsedDuration)) {
      form.duration = parsedDuration
    }
  }

  const frequencyValue = Number(frequency)
  if (Number.isFinite(frequencyValue)) {
    dayLabels.forEach((day) => {
      form[day.key] = (frequencyValue & day.bit) !== 0
    })
  }
}

function toIsoDateTime(value) {
  return value ? new Date(value).toISOString() : null
}

function isValidGuid(value) {
  // Validación flexible: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
  // Acepta cualquier GUID aunque no sea RFC 4122 estricto
  return typeof value === 'string' && /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(value)
}

function validate() {
  error.value = ''
  console.log('🔍 Validando formulario...', form)
  
  if (!form.status) {
    console.warn('❌ Estado no seleccionado')
    return 'El estado es requerido'
  }
  if (!form.flightDate) return 'La fecha del vuelo es requerida'
  if (!form.realDepartureTime) return 'La hora real de salida es requerida'
  if (!form.realArrivalTime) return 'La hora real de llegada es requerida'
  if (!form.checkInStartTime) return 'La hora de inicio de check-in es requerida'
  if (!form.checkInDeadline) return 'La hora límite de check-in es requerida'
  if (form.duration <= 0) return 'La duración debe ser mayor a 0'
  
  console.log('🛩️ Validando aircraftId:', form.aircraftId, 'Tipo:', typeof form.aircraftId)
  if (!form.aircraftId) {
    console.warn('❌ AircraftId está vacío')
    return 'El id de la aeronave es requerido'
  }
  
  const isValidAircraft = isValidGuid(form.aircraftId)
  console.log('✅ ¿Es GUID válido?', isValidAircraft)
  if (!isValidAircraft) {
    console.warn('❌ AircraftId no es GUID válido:', form.aircraftId)
    return 'La aeronave seleccionada debe entregar un GUID válido'
  }
  
  if (form.itineraryId <= 0) return 'El itinerario es requerido'

  if (!form.businessId) return 'El business id es requerido'

  console.log('👤 Validando businessId:', form.businessId, 'Tipo:', typeof form.businessId)
  const isValidBusiness = isValidGuid(form.businessId)
  console.log('✅ ¿BusinessId es GUID válido?', isValidBusiness, 'Validador usado:', /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/)
  if (!isValidBusiness) {
    console.warn('❌ BusinessId no es GUID válido:', form.businessId)
    return 'El business id debe ser un GUID válido'
  }
  if (form.flightRouteId <= 0) return 'Selecciona una ruta de vuelo antes de continuar'
  if (!form.departureAirportCode) return 'La ruta seleccionada no tiene aeropuerto de salida'
  if (!form.arrivalAirportCode) return 'La ruta seleccionada no tiene aeropuerto de llegada'
  if (form.departureAirportCode === form.arrivalAirportCode) return 'La salida y la llegada no pueden ser el mismo aeropuerto'
  if (!form.monday && !form.tuesday && !form.wednesday && !form.thursday && !form.friday && !form.saturday && !form.sunday) {
    return 'La ruta seleccionada no tiene dias de disponibilidad'
  }
  
  console.log('✅ Validación pasada')
  return ''
}

async function submit() {
  console.log('📋 Submit iniciado')
  const v = validate()
  if (v) {
    console.error('❌ Validación falló:', v)
    error.value = v
    return
  }

  const payload = {
    Status: form.status,
    FlightDate: toIsoDateTime(form.flightDate),
    TouristPrice: Number(form.touristPrice),
    FirstClassPrice: Number(form.firstClassPrice),
    RealDepartureTime: toIsoDateTime(form.realDepartureTime),
    RealArrivalTime: toIsoDateTime(form.realArrivalTime),
    CheckInStartTime: toIsoDateTime(form.checkInStartTime),
    CheckInDeadline: toIsoDateTime(form.checkInDeadline),
    Duration: Number(form.duration),
    AirlineId: Number(form.airlineId),
    AircraftId: form.aircraftId,
    ItineraryId: Number(form.itineraryId),
    BusinessId: form.businessId,
    FlightRouteId: Number(form.flightRouteId),
    AvailableSeats: Number(form.availableSeats),
    CarryOnPrice: form.carryOnPrice ? Number(form.carryOnPrice) : null,
    CheckedPrice: form.checkedPrice ? Number(form.checkedPrice) : null,
    DepartureAirportCode: form.departureAirportCode,
    ArrivalAirportCode: form.arrivalAirportCode,
    Monday: form.monday,
    Tuesday: form.tuesday,
    Wednesday: form.wednesday,
    Thursday: form.thursday,
    Friday: form.friday,
    Saturday: form.saturday,
    Sunday: form.sunday,
    Wifi: form.wifi,
    Entertainment: form.entertainment,
    Food: form.food,
    SeatSelection: form.seatSelection,
  }
  
  console.log('📦 Payload enviado:', JSON.stringify(payload, null, 2))
  console.log('🔎 BusinessId en payload:', payload.BusinessId)

  try {
    console.log('🔄 Enviando POST a:', flightCreateUrl)
    const res = await axios.post(flightCreateUrl, payload)
    console.log('✅ Respuesta exitosa:', res.data)
    message.value = res?.data?.message || 'Vuelo creado correctamente'
    error.value = ''
  } catch (e) {
    console.error('❌ Error en POST:', e)
    console.error('Status:', e?.response?.status)
    console.error('Datos error:', e?.response?.data)
    if (e?.response?.data) {
      error.value = JSON.stringify(e.response.data)
    } else {
      error.value = 'Error al crear el vuelo'
    }
  }
}
</script>

<template>
  <PublicNavBar />
  <main class="mx-auto flex min-h-[calc(100svh-4rem)] w-full max-w-7xl flex-col gap-6 px-4 py-6 lg:px-8">
    <section class="rounded-3xl bg-white p-6 shadow-[0_12px_40px_rgba(15,23,42,0.08)] ring-1 ring-slate-200">
      <div class="mb-6">
        <p class="text-sm font-semibold uppercase tracking-[0.2em] text-primary">Módulo de vuelos</p>
        <h2 class="mt-2 text-3xl font-semibold text-slate-900">Crear vuelo</h2>

      </div>

      <form @submit.prevent="submit" class="grid gap-6">
        <div class="rounded-2xl border border-slate-200 bg-slate-50 p-4">
          <div class="mb-2 flex flex-wrap items-center justify-between gap-2">
            <p class="text-sm font-semibold text-slate-800">Ruta seleccionada</p>
            <router-link
              class="rounded-lg border border-primary px-3 py-1 text-xs font-semibold text-primary transition hover:bg-primary hover:text-white"
              :to="{ name: 'routes' }"
            >
              Cambiar ruta
            </router-link>
          </div>
          <div v-if="hasSelectedRoute" class="grid gap-2 text-sm text-slate-700 sm:grid-cols-2">
            <p><span class="font-semibold">Origen:</span> {{ form.departureAirportCode || '-' }}</p>
            <p><span class="font-semibold">Destino:</span> {{ form.arrivalAirportCode || '-' }}</p>
            <p><span class="font-semibold">Duracion:</span> {{ formatDuration(form.duration) }}</p>
            <p><span class="font-semibold">Dias disponibles:</span> {{ selectedDaysText }}</p>
          </div>
          <div v-else class="text-sm text-slate-600">
            Selecciona una ruta para prellenar origen, destino, duracion y dias disponibles.
            <router-link class="ml-1 font-semibold text-primary hover:underline" :to="{ name: 'routes' }">
              Ir a rutas
            </router-link>
          </div>
        </div>

        <div class="grid gap-6 lg:grid-cols-2">
          <LandingDropdownField
            v-model="form.status"
            label="Estado"
            placeholder="Selecciona un estado"
            :options="statusOptions"
            button-label="Mostrar estados"
          />

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Aeropuerto salida
            <input
              :value="form.departureAirportCode"
              type="text"
              readonly
              class="rounded-xl border border-slate-300 bg-slate-100 px-3 py-2.5 text-sm text-slate-900 outline-none"
            />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Fecha del vuelo
            <input v-model="form.flightDate" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Aeropuerto llegada
            <input
              :value="form.arrivalAirportCode"
              type="text"
              readonly
              class="rounded-xl border border-slate-300 bg-slate-100 px-3 py-2.5 text-sm text-slate-900 outline-none"
            />
          </label>


          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Hora real de salida
            <input v-model="form.realDepartureTime" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <LandingDropdownField
            v-model="form.aircraftId"
            label="Tipo de aeronave"
            placeholder="Selecciona una aeronave"
            :options="aircraftOptions"
            button-label="Mostrar aeronaves"
          />

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Hora real de llegada
            <input v-model="form.realArrivalTime" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Inicio de check-in
            <input v-model="form.checkInStartTime" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Límite de check-in
            <input v-model="form.checkInDeadline" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>


          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Asientos disponibles
            <input v-model.number="form.availableSeats" type="number" min="0" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Precio turista
            <input v-model.number="form.touristPrice" type="number" min="0" step="0.01" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Precio primera clase
            <input v-model.number="form.firstClassPrice" type="number" min="0" step="0.01" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Precio carry-on
            <input v-model.number="form.carryOnPrice" type="number" min="0" step="0.01" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Precio checked
            <input v-model.number="form.checkedPrice" type="number" min="0" step="0.01" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>


          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Itinerario (id)
            <input v-model.number="form.itineraryId" type="number" min="1" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>


          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Business id (GUID)
            <input v-model="form.businessId" type="text" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>


          <fieldset class="lg:col-span-2 rounded-2xl border border-slate-200 bg-slate-50 p-4">
            <p class="text-sm font-semibold text-slate-800 mb-3">Servicios incluidos en el vuelo</p>
            <div class="mt-3 grid grid-cols-2 gap-3 sm:grid-cols-4">
              <label class="inline-flex items-center gap-2 text-sm text-slate-700">
                <input v-model="form.wifi" type="checkbox" class="h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary/30" />
                WiFi
              </label>
              <label class="inline-flex items-center gap-2 text-sm text-slate-700">
                <input v-model="form.entertainment" type="checkbox" class="h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary/30" />
                Entretenimiento
              </label>
              <label class="inline-flex items-center gap-2 text-sm text-slate-700">
                <input v-model="form.food" type="checkbox" class="h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary/30" />
                Alimentación
              </label>
              <label class="inline-flex items-center gap-2 text-sm text-slate-700">
                <input v-model="form.seatSelection" type="checkbox" class="h-4 w-4 rounded border-slate-300 text-primary focus:ring-primary/30" />
                Selección de asiento
              </label>
            </div>
          </fieldset>
        </div>

        <div>
          <div v-if="message" class="mb-4 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{{ message }}</div>
          <div v-if="error" class="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 wrap-break-word">{{ error }}</div>
        </div>

        <div class="flex justify-end">
          <button type="submit" class="rounded-xl bg-primary px-6 py-3 text-sm font-semibold text-white shadow-[0_10px_24px_rgba(113,23,23,0.28)] transition hover:bg-select">
            Crear vuelo
          </button>
        </div>
      </form>
    </section>
  </main>
</template>
