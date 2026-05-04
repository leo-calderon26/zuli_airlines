<script setup>
import HeaderComponent from '../../../shared/HeaderComponent.vue'
import axios from 'axios'
import { reactive, ref } from 'vue'

const baseUrl = 'http://localhost:5001'

const form = reactive({
  status: '',
  flightDate: '',
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
})

const message = ref('')
const error = ref('')

function validate() {
  error.value = ''
  if (!form.status) return 'El estado es requerido'
  if (!form.flightDate) return 'La fecha del vuelo es requerida'
  if (form.duration <= 0) return 'La duración debe ser mayor a 0'
  if (form.airlineId <= 0) return 'La aerolínea es requerida'
  if (!form.aircraftId) return 'El id de la aeronave es requerido'
  if (form.itineraryId <= 0) return 'El itinerario es requerido'
  if (!form.adminId) return 'El id del administrador es requerido'
  if (form.flightRouteId <= 0) return 'La ruta de vuelo es requerida'
  return ''
}

async function submit() {
  const v = validate()
  if (v) { error.value = v; return }

  // Map to backend DTO shape
  const payload = {
    Status: form.status,
    FlightDate: new Date(form.flightDate).toISOString(),
    TouristPrice: Number(form.touristPrice),
    FirstClassPrice: Number(form.firstClassPrice),
    Duration: Number(form.duration),
    AirlineId: Number(form.airlineId),
    AircraftId: form.aircraftId,
    ItineraryId: Number(form.itineraryId),
    AdminId: form.adminId,
    FlightRouteId: Number(form.flightRouteId),
    AvailableSeats: Number(form.availableSeats),
    CarryOnPrice: form.carryOnPrice ? Number(form.carryOnPrice) : null,
    CheckedPrice: form.checkedPrice ? Number(form.checkedPrice) : null,
  }

  try {
    const res = await axios.post(`${baseUrl}/api/Flight/Create`, payload)
    message.value = res?.data?.message || 'Vuelo creado correctamente'
    error.value = ''
  } catch (e) {
    console.error(e)
    if (e?.response?.data) {
      error.value = JSON.stringify(e.response.data)
    } else {
      error.value = 'Error al crear el vuelo'
    }
  }
}
</script>

<template>
  <HeaderComponent />
  <main class="p-4">
    <h2 class="text-2xl font-semibold mb-4">Crear Vuelo</h2>

    <div v-if="message" class="text-green-600 mb-2">{{ message }}</div>
    <div v-if="error" class="text-red-600 mb-2 wrap-break-word">{{ error }}</div>

    <form @submit.prevent="submit" class="grid gap-3 max-w-xl">
      <label class="flex flex-col">Estado
        <input v-model="form.status" type="text" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Fecha y hora del vuelo
        <input v-model="form.flightDate" type="datetime-local" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Duración (minutos)
        <input v-model.number="form.duration" type="number" min="1" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Precio turista
        <input v-model.number="form.touristPrice" type="number" step="0.01" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Precio primera clase
        <input v-model.number="form.firstClassPrice" type="number" step="0.01" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Aerolínea (id)
        <input v-model.number="form.airlineId" type="number" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Id de aeronave (GUID)
        <input v-model="form.aircraftId" type="text" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Itinerario (id)
        <input v-model.number="form.itineraryId" type="number" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Id administrador (GUID)
        <input v-model="form.adminId" type="text" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Ruta de vuelo (id)
        <input v-model.number="form.flightRouteId" type="number" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Asientos disponibles
        <input v-model.number="form.availableSeats" type="number" min="0" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Precio Carry-on (opcional)
        <input v-model.number="form.carryOnPrice" type="number" step="0.01" class="border rounded px-2 py-1 mt-1" />
      </label>

      <label class="flex flex-col">Precio Checked (opcional)
        <input v-model.number="form.checkedPrice" type="number" step="0.01" class="border rounded px-2 py-1 mt-1" />
      </label>

      <button type="submit" class="bg-blue-600 text-white px-4 py-2 rounded mt-2">Crear vuelo</button>
    </form>
  </main>
</template>
