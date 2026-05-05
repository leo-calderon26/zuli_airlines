<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue'
import LandingDropdownField from '../../landing/components/LandingDropdownField.vue'
import axios from 'axios'
import { computed, onMounted, reactive, ref } from 'vue'
import { useAircraft } from '../../aircraft/composable/useAircraft'
import { useAirport } from '../../airport/composable/useAirport'

const flightCreateUrl = '/api/Flight/Create'
const { fetchAircrafts, aircrafts } = useAircraft()
const { fetchAirports, airports } = useAirport()

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
  airlineId: 0,
  aircraftId: '',
  itineraryId: 0,
  adminId: '',
  flightRouteId: 0,
  availableSeats: 0,
  carryOnPrice: null,
  checkedPrice: null,
  departureAirportCode: '',
  arrivalAirportCode: '',
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

const airportOptions = computed(() => (airports.value ?? [])
  .map((airport) => ({
    label: `${airport.airportCode} - ${airport.city}`,
    value: airport.airportCode
  }))
  .filter((airport) => airport.value !== ''))

onMounted(async () => {
  console.log('🚀 FlightCreate mounted')
  await fetchAircrafts()
  console.log('✈️ Aircrafts loaded:', aircrafts.value)
  await fetchAirports()
  console.log('🏠 Airports loaded:', airports.value)
})

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
  if (form.airlineId <= 0) return 'La aerolínea es requerida'
  
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
  if (!form.adminId) return 'El id del administrador es requerido'
  
  console.log('👤 Validando adminId:', form.adminId, 'Tipo:', typeof form.adminId)
  const isValidAdmin = isValidGuid(form.adminId)
  console.log('✅ ¿AdminId es GUID válido?', isValidAdmin, 'Validador usado:', /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/)
  if (!isValidAdmin) {
    console.warn('❌ AdminId no es GUID válido:', form.adminId)
    return 'El id del administrador debe ser un GUID válido'
  }
  if (form.flightRouteId <= 0) return 'La ruta de vuelo es requerida'
  if (!form.departureAirportCode) return 'El aeropuerto de salida es requerido'
  if (!form.arrivalAirportCode) return 'El aeropuerto de llegada es requerido'
  if (form.departureAirportCode === form.arrivalAirportCode) return 'La salida y la llegada no pueden ser el mismo aeropuerto'
  
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
    AdminId: form.adminId,
    FlightRouteId: Number(form.flightRouteId),
    AvailableSeats: Number(form.availableSeats),
    CarryOnPrice: form.carryOnPrice ? Number(form.carryOnPrice) : null,
    CheckedPrice: form.checkedPrice ? Number(form.checkedPrice) : null,
    DepartureAirportCode: form.departureAirportCode,
    ArrivalAirportCode: form.arrivalAirportCode,
  }
  
  console.log('📦 Payload enviado:', JSON.stringify(payload, null, 2))
  console.log('🔎 AdminId en payload:', payload.AdminId)

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

      <div v-if="message" class="mb-4 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{{ message }}</div>
      <div v-if="error" class="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 wrap-break-word">{{ error }}</div>

      <form @submit.prevent="submit" class="grid gap-6">
        
        <div class="grid gap-6 lg:grid-cols-2">
          <LandingDropdownField
            v-model="form.status"
            label="Estado"
            placeholder="Selecciona un estado"
            :options="statusOptions"
            button-label="Mostrar estados"
          />

          <LandingDropdownField
            v-model="form.departureAirportCode"
            label="Aeropuerto salida"
            placeholder="Selecciona un aeropuerto"
            :options="airportOptions"
            button-label="Mostrar aeropuertos"
          />

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Fecha del vuelo
            <input v-model="form.flightDate" type="datetime-local" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <LandingDropdownField
            v-model="form.arrivalAirportCode"
            label="Aeropuerto llegada"
            placeholder="Selecciona un aeropuerto"
            :options="airportOptions"
            button-label="Mostrar aeropuertos"
          />

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

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Duración
            <input v-model.number="form.duration" type="number" min="1" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
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

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Aerolínea (id)
            <input v-model.number="form.airlineId" type="number" min="1" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Itinerario (id)
            <input v-model.number="form.itineraryId" type="number" min="1" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Ruta de vuelo (id)
            <input v-model.number="form.flightRouteId" type="number" min="1" step="1" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>

          <label class="flex flex-col gap-1 text-sm font-medium text-slate-700">Id administrador (GUID)
            <input v-model="form.adminId" type="text" class="rounded-xl border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/15" />
          </label>
        </div>

        <div class="rounded-2xl bg-slate-50 px-4 py-3 text-sm text-slate-600 ring-1 ring-slate-200">
          <p class="font-semibold text-slate-700">Notas</p>
          <p class="mt-1">Aeropuerto salida y llegada muestran código + ciudad. La aeronave se toma del listado de aeronaves disponibles.</p>
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
