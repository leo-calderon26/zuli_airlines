<script setup>
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { createFlightRoute, searchAirportSuggestionsByName } from '../service/routeService';
import PublicNavBar from '../../../shared/PublicNavBar.vue';

const router = useRouter();

const form = reactive({
  origin: '',
  destination: '',
  scheduledDepartureDay: '',
  scheduledDepartureMonth: '',
  scheduledDepartureTime: '',
  scheduledArrivalDay: '',
  scheduledArrivalMonth: '',
  scheduledArrivalTime: '',
  duration: '',
  frequency: [],
});

const errors = reactive({
  global: '',
  fields: {},
});

const originSuggestions = ref([]);
const destinationSuggestions = ref([]);
let originSearchTimer = null;
let destinationSearchTimer = null;
let latestOriginTerm = '';
let latestDestinationTerm = '';

const daysOfWeek = [
  { value: 'mon', label: 'Lunes' },
  { value: 'tue', label: 'Martes' },
  { value: 'wed', label: 'Miercoles' },
  { value: 'thu', label: 'Jueves' },
  { value: 'fri', label: 'Viernes' },
  { value: 'sat', label: 'Sabado' },
  { value: 'sun', label: 'Domingo' },
];

const dayBits = {
  mon: 1,
  tue: 2,
  wed: 4,
  thu: 8,
  fri: 16,
  sat: 32,
  sun: 64,
};

const airportCodePattern = /^[A-Za-z0-9]{3}$/;
// TODO(Randy) pegar esto con la parte del admin
const ADMIN_ID_COOKIE = 'adminId';
const ADMIN_ID = 'ZULI-ADMIN-001';
const AIRLINE_ID = 1;

function normalizeCode(value) {
  return String(value || '').toUpperCase().trim();
}

function normalizeTerm(value) {
  return String(value || '').trim();
}

function getSuggestionCode(suggestion) {
  return suggestion?.airportCode || suggestion?.AirportCode || '';
}

function getSuggestionLabel(suggestion) {
  const displayName = suggestion?.displayName || suggestion?.DisplayName || '';
  if (displayName) return displayName;
  const name = suggestion?.name || suggestion?.Name || '';
  const country = suggestion?.country || suggestion?.Country || '';

  if (name && country) return `${name}, ${country}`;
  if (name) return name;
  return country;
}

function clearSuggestionList(field) {
  if (field === 'origin') {
    originSuggestions.value = [];
    return;
  }
  destinationSuggestions.value = [];
}

function clearSuggestionListOnBlur(field) {
  setTimeout(() => clearSuggestionList(field), 150);
}

function cancelSearchTimer(field) {
  if (field === 'origin' && originSearchTimer) {
    clearTimeout(originSearchTimer);
    originSearchTimer = null;
  }

  if (field === 'destination' && destinationSearchTimer) {
    clearTimeout(destinationSearchTimer);
    destinationSearchTimer = null;
  }
}

async function fetchSuggestions(term, field) {
  try {
    const results = await searchAirportSuggestionsByName(term);
    if (field === 'origin') {
      if (term !== latestOriginTerm) return;
      originSuggestions.value = Array.isArray(results) ? results : [];
      return;
    }

    if (term !== latestDestinationTerm) return;
    destinationSuggestions.value = Array.isArray(results) ? results : [];
  } catch (error) {
    console.error('Error al buscar aeropuertos:', error);
  }
}

function handleAirportInput(field) {
  form[field] = normalizeTerm(form[field]);
  cancelSearchTimer(field);

  const term = form[field];
  if (term.length < 3) {
    clearSuggestionList(field);
    return;
  }

  if (field === 'origin') {
    latestOriginTerm = term;
    originSearchTimer = setTimeout(() => fetchSuggestions(term, field), 1000);
    return;
  }

  latestDestinationTerm = term;
  destinationSearchTimer = setTimeout(() => fetchSuggestions(term, field), 1000);
}

function applySuggestion(field, suggestion) {
  const code = getSuggestionCode(suggestion);
  if (!code) return;
  form[field] = normalizeCode(code);
  clearSuggestionList(field);
}

function isIntegerLike(value) {
  return Number.isInteger(Number(value)) && String(value) !== '';
}

function getCookieValue(name) {
  if (typeof document === 'undefined') return '';
  const matches = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return matches ? decodeURIComponent(matches[1]) : '';
}

function getAdminId() {
  return getCookieValue(ADMIN_ID_COOKIE) || sessionStorage.getItem('businessId') || ADMIN_ID;
}

function isValidDateTime(value) {
  return Boolean(value) && !Number.isNaN(Date.parse(value));
}

function toIsoDateTime(value) {
  if (!isValidDateTime(value)) return '';
  return value.length === 16 ? `${value}:00` : value;
}

function isValidDayMonth(day, month) {
  const dayNum = Number(day);
  const monthNum = Number(month);
  return Number.isInteger(dayNum) && dayNum >= 1 && dayNum <= 31 && Number.isInteger(monthNum) && monthNum >= 1 && monthNum <= 12;
}

function buildDateTimeFromParts(day, month, time) {
  if (!isValidDayMonth(day, month) || !time) return '';
  const year = new Date().getFullYear();
  const monthPadded = String(month).padStart(2, '0');
  const dayPadded = String(day).padStart(2, '0');
  return `${year}-${monthPadded}-${dayPadded}T${time}`;
}

function encodeDays(selectedDays) {
  if (!Array.isArray(selectedDays)) return 0;
  return selectedDays.reduce((acc, day) => acc | (dayBits[day] || 0), 0);
}

function decodeDays(mask) {
  return Object.keys(dayBits).filter((day) => (mask & dayBits[day]) !== 0);
}

function formatFrequencyFromMask(mask) {
  const selected = decodeDays(mask);
  if (selected.length === 0) return 'Sin frecuencia';
  return daysOfWeek
    .filter((day) => selected.includes(day.value))
    .map((day) => day.label)
    .join(', ');
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

  const departureDateTime = buildDateTimeFromParts(
    form.scheduledDepartureDay,
    form.scheduledDepartureMonth,
    form.scheduledDepartureTime
  );

  const arrivalDateTime = buildDateTimeFromParts(
    form.scheduledArrivalDay,
    form.scheduledArrivalMonth,
    form.scheduledArrivalTime
  );

  if (!isValidDateTime(departureDateTime)) {
    errors.fields.scheduledDepartureTime = 'Fecha y hora de salida invalidas';
  }

  if (!isValidDateTime(arrivalDateTime)) {
    errors.fields.scheduledArrivalTime = 'Fecha y hora de llegada invalidas';
  }

  if (
    form.duration === '' ||
    form.duration === null ||
    isNaN(Number(form.duration)) ||
    !isIntegerLike(form.duration) ||
    Number(form.duration) <= 0 ||
    Number(form.duration) > 1140
  ) {
    errors.fields.duration = 'Duracion invalida (maximo 1140 minutos)';
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

  const adminId = getAdminId();

  if (!adminId) {
    errors.global = 'No se pudo obtener el adminId desde las cookies.';
    alert(errors.global);
    return;
  }

  const routePayload = {
    frequency: encodeDays(form.frequency),
    scheduledArrivalTime: toIsoDateTime(
      buildDateTimeFromParts(form.scheduledArrivalDay, form.scheduledArrivalMonth, form.scheduledArrivalTime)
    ),
    scheduledDepartureTime: toIsoDateTime(
      buildDateTimeFromParts(
        form.scheduledDepartureDay,
        form.scheduledDepartureMonth,
        form.scheduledDepartureTime
      )
    ),
    estimatedDuration: Number(form.duration) * 60,
    businessId: adminId,
    airlineId: AIRLINE_ID,
    arrivalAirport: normalizeCode(form.destination),
    departureAirport: normalizeCode(form.origin),
  };

  try {
    await createFlightRoute(routePayload);
    router.push({ name: 'mainMenu' });
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
            <div class="form-field group" :class="{ 'has-suggestions': originSuggestions.length }">
              <input
                id="origin"
                v-model="form.origin"
                name="origin"
                type="text"
                maxlength="40"
                class="form-input peer"
                placeholder=" "
                autocomplete="off"
                @input="handleAirportInput('origin')"
                @blur="clearSuggestionListOnBlur('origin')"
              />
              <label for="origin" class="form-label">Aeropuerto origen</label>
              <div v-if="originSuggestions.length" class="suggestion-list">
                <button
                  v-for="suggestion in originSuggestions"
                  :key="getSuggestionCode(suggestion)"
                  type="button"
                  class="suggestion-item"
                  @mousedown.prevent="applySuggestion('origin', suggestion)"
                >
                  <span class="suggestion-code">{{ getSuggestionCode(suggestion) }}</span>
                  <span class="suggestion-label">{{ getSuggestionLabel(suggestion) }}</span>
                </button>
              </div>
              <p v-if="errors.fields.origin" class="text-sm text-error">{{ errors.fields.origin }}</p>
            </div>

            <div class="form-field group" :class="{ 'has-suggestions': destinationSuggestions.length }">
              <input
                id="destination"
                v-model="form.destination"
                name="destination"
                type="text"
                maxlength="40"
                class="form-input peer"
                placeholder=" "
                autocomplete="off"
                @input="handleAirportInput('destination')"
                @blur="clearSuggestionListOnBlur('destination')"
              />
              <label for="destination" class="form-label">Aeropuerto destino</label>
              <div v-if="destinationSuggestions.length" class="suggestion-list">
                <button
                  v-for="suggestion in destinationSuggestions"
                  :key="getSuggestionCode(suggestion)"
                  type="button"
                  class="suggestion-item"
                  @mousedown.prevent="applySuggestion('destination', suggestion)"
                >
                  <span class="suggestion-code">{{ getSuggestionCode(suggestion) }}</span>
                  <span class="suggestion-label">{{ getSuggestionLabel(suggestion) }}</span>
                </button>
              </div>
              <p v-if="errors.fields.destination" class="text-sm text-error">{{ errors.fields.destination }}</p>
            </div>

            <div class="form-field group">
              <div class="grid grid-cols-1 gap-3 sm:grid-cols-3">
                <input
                  id="scheduledDepartureDay"
                  v-model="form.scheduledDepartureDay"
                  name="scheduledDepartureDay"
                  type="number"
                  min="1"
                  max="31"
                  step="1"
                  class="form-input peer"
                  placeholder="Dia"
                />
                <input
                  id="scheduledDepartureMonth"
                  v-model="form.scheduledDepartureMonth"
                  name="scheduledDepartureMonth"
                  type="number"
                  min="1"
                  max="12"
                  step="1"
                  class="form-input peer"
                  placeholder="Mes"
                />
                <input
                  id="scheduledDepartureTime"
                  v-model="form.scheduledDepartureTime"
                  name="scheduledDepartureTime"
                  type="time"
                  step="60"
                  class="form-input peer"
                  placeholder="Hora"
                />
              </div>
              <label class="form-label">Salida programada (dia, mes, hora)</label>
              <p v-if="errors.fields.scheduledDepartureTime" class="text-sm text-error">
                {{ errors.fields.scheduledDepartureTime }}
              </p>
            </div>

            <div class="form-field group">
              <div class="grid grid-cols-1 gap-3 sm:grid-cols-3">
                <input
                  id="scheduledArrivalDay"
                  v-model="form.scheduledArrivalDay"
                  name="scheduledArrivalDay"
                  type="number"
                  min="1"
                  max="31"
                  step="1"
                  class="form-input peer"
                  placeholder="Dia"
                />
                <input
                  id="scheduledArrivalMonth"
                  v-model="form.scheduledArrivalMonth"
                  name="scheduledArrivalMonth"
                  type="number"
                  min="1"
                  max="12"
                  step="1"
                  class="form-input peer"
                  placeholder="Mes"
                />
                <input
                  id="scheduledArrivalTime"
                  v-model="form.scheduledArrivalTime"
                  name="scheduledArrivalTime"
                  type="time"
                  step="60"
                  class="form-input peer"
                  placeholder="Hora"
                />
              </div>
              <label class="form-label">Llegada programada (dia, mes, hora)</label>
              <p v-if="errors.fields.scheduledArrivalTime" class="text-sm text-error">
                {{ errors.fields.scheduledArrivalTime }}
              </p>
            </div>

            <div class="form-field group">
              <input
                id="duration"
                v-model="form.duration"
                name="duration"
                type="number"
                min="1"
                max="1140"
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

.has-suggestions {
  @apply pb-44;
}

.suggestion-list {
  @apply absolute z-20 mt-2 w-full rounded-md border border-gray-200 bg-white shadow-lg;
}

.suggestion-item {
  @apply flex w-full items-start gap-2 px-3 py-2 text-left text-sm text-gray-900 hover:bg-gray-100;
}

.suggestion-code {
  @apply font-semibold text-gray-900;
}

.suggestion-label {
  @apply text-gray-600;
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