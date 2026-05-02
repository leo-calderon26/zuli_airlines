<script setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import PublicNavBar from '../../../shared/PublicNavBar.vue';

const router = useRouter();

const form = reactive({
  origin: '',
  destination: '',
  departureTime: '',
  arrivalTime: '',
  duration: '',
  frequency: [],
});

const errors = reactive({
  global: '',
  fields: {},
});

const daysOfWeek = [
  { value: 'mon', label: 'Lunes' },
  { value: 'tue', label: 'Martes' },
  { value: 'wed', label: 'Miercoles' },
  { value: 'thu', label: 'Jueves' },
  { value: 'fri', label: 'Viernes' },
  { value: 'sat', label: 'Sabado' },
  { value: 'sun', label: 'Domingo' },
];

const airportCodePattern = /^[A-Za-z0-9]{3}$/;
const utcTimePattern = /^([01]\d|2[0-3]):[0-5]\d$/;

function normalizeCode(value) {
  return String(value || '').toUpperCase().trim();
}

function isIntegerLike(value) {
  return Number.isInteger(Number(value)) && String(value) !== '';
}

function validate() {
  errors.global = '';
  errors.fields = {};

  const origin = normalizeCode(form.origin);
  const destination = normalizeCode(form.destination);

  if (!origin || !airportCodePattern.test(origin)) {
    errors.fields.origin = 'Origen invalido (3 caracteres, sin especiales)';
  }

  if (!destination || !airportCodePattern.test(destination)) {
    errors.fields.destination = 'Destino invalido (3 caracteres, sin especiales)';
  }

  if (origin && destination && origin === destination) {
    errors.fields.destination = 'El origen y el destino no pueden ser iguales';
  }

  if (!form.departureTime || !utcTimePattern.test(form.departureTime)) {
    errors.fields.departureTime = 'Hora de salida invalida (UTC HH:MM)';
  }

  if (!form.arrivalTime || !utcTimePattern.test(form.arrivalTime)) {
    errors.fields.arrivalTime = 'Hora de llegada invalida (UTC HH:MM)';
  }

  if (
    form.duration === '' ||
    form.duration === null ||
    isNaN(Number(form.duration)) ||
    !isIntegerLike(form.duration) ||
    Number(form.duration) <= 0 ||
    Number(form.duration) > 99
  ) {
    errors.fields.duration = 'Duracion invalida (maximo 2 digitos)';
  }

  if (!Array.isArray(form.frequency) || form.frequency.length === 0) {
    errors.fields.frequency = 'Debe seleccionar al menos un dia';
  }

  return Object.keys(errors.fields).length === 0 && errors.global === '';
}

async function handleSubmit() {
  if (!validate()) {
    if (errors.global) alert(errors.global);
    return;
  }

  const routePayload = {
    origin: normalizeCode(form.origin),
    destination: normalizeCode(form.destination),
    departureTime: form.departureTime,
    arrivalTime: form.arrivalTime,
    duration: Number(form.duration),
    frequency: [...form.frequency],
  };

  try {
    console.log('Route payload', routePayload);
    alert('La ruta se ha creado correctamente');
    router.push({ name: 'routes' });
  } catch (error) {
    errors.global = error.response?.data?.message || 'Error al crear la ruta';
    alert(errors.global);
  }
}
</script>

<template>
  <div class="flex flex-col">
    <PublicNavBar />
    <main class="flex-1 pb-8">
      <div class="page-shell">
        <form class="form-card" @submit.prevent="handleSubmit">
          <div class="form-grid">
            <div class="form-field group">
              <input
                id="origin"
                v-model="form.origin"
                name="origin"
                type="text"
                maxlength="3"
                class="form-input peer"
                placeholder=" "
                @input="form.origin = normalizeCode(form.origin)"
              />
              <label for="origin" class="form-label">Aeropuerto origen</label>
              <p v-if="errors.fields.origin" class="text-sm text-error">{{ errors.fields.origin }}</p>
            </div>

            <div class="form-field group">
              <input
                id="destination"
                v-model="form.destination"
                name="destination"
                type="text"
                maxlength="3"
                class="form-input peer"
                placeholder=" "
                @input="form.destination = normalizeCode(form.destination)"
              />
              <label for="destination" class="form-label">Aeropuerto destino</label>
              <p v-if="errors.fields.destination" class="text-sm text-error">{{ errors.fields.destination }}</p>
            </div>

            <div class="form-field group">
              <input
                id="departureTime"
                v-model="form.departureTime"
                name="departureTime"
                type="time"
                step="60"
                class="form-input peer"
                placeholder=" "
              />
              <label for="departureTime" class="form-label">Hora de salida (UTC)</label>
              <p v-if="errors.fields.departureTime" class="text-sm text-error">{{ errors.fields.departureTime }}</p>
            </div>

            <div class="form-field group">
              <input
                id="arrivalTime"
                v-model="form.arrivalTime"
                name="arrivalTime"
                type="time"
                step="60"
                class="form-input peer"
                placeholder=" "
              />
              <label for="arrivalTime" class="form-label">Hora de llegada (UTC)</label>
              <p v-if="errors.fields.arrivalTime" class="text-sm text-error">{{ errors.fields.arrivalTime }}</p>
            </div>

            <div class="form-field group">
              <input
                id="duration"
                v-model="form.duration"
                name="duration"
                type="number"
                min="1"
                max="99"
                step="1"
                class="form-input peer"
                placeholder=" "
              />
              <label for="duration" class="form-label">Duracion del vuelo (min)</label>
              <p v-if="errors.fields.duration" class="text-sm text-error">{{ errors.fields.duration }}</p>
            </div>

            <div class="form-field group">
              <div class="frequency-label">Frecuencia semanal</div>
              <div class="frequency-grid">
                <label v-for="day in daysOfWeek" :key="day.value" class="frequency-item">
                  <input
                    v-model="form.frequency"
                    type="checkbox"
                    :value="day.value"
                    class="frequency-checkbox"
                  />
                  <span>{{ day.label }}</span>
                </label>
              </div>
              <p v-if="errors.fields.frequency" class="text-sm text-error">{{ errors.fields.frequency }}</p>
            </div>
          </div>

          <div class="mt-4">
            <button type="submit" class="submit-btn">Guardar ruta</button>
          </div>
        </form>
      </div>
    </main>
  </div>
  <PublicBottomBar />
</template>

<style scoped>
@reference "../../../style.css";

.page-shell {
  @apply mx-auto w-full max-w-5xl px-6;
}

.form-card {
  @apply w-full max-w-none rounded-lg border border-gray-200 bg-white p-8 shadow-sm;
}

.form-grid {
  @apply grid grid-cols-1 gap-6 md:grid-cols-2;
}

.form-field {
  @apply relative z-0 w-full;
}

.form-input {
  @apply block w-full appearance-none border-0 border-b-2 border-gray-300 bg-transparent px-0 py-3 text-base
    text-gray-900 focus:border-gold focus:outline-none focus:ring-0;
}

.form-label {
  @apply absolute top-3 -z-10 origin-[0] -translate-y-6 transform text-base
    text-gray-600 duration-300 peer-placeholder-shown:translate-y-0 peer-placeholder-shown:scale-100
    peer-focus:start-0 peer-focus:-translate-y-6 peer-focus:text-gold;
}

.frequency-label {
  @apply text-base text-gray-700;
}

.frequency-grid {
  @apply mt-3 grid grid-cols-2 gap-2 sm:grid-cols-3;
}

.frequency-item {
  @apply flex items-center gap-2 text-sm text-gray-700;
}

.frequency-checkbox {
  @apply h-4 w-4 rounded border-gray-300 text-gold focus:ring-gold;
}

.submit-btn {
  @apply mt-2 inline-flex rounded-md border border-transparent bg-primary px-5 py-3 text-base font-medium
    text-white hover:bg-select focus:outline-none focus:ring-2 focus:ring-gold;
}
</style>