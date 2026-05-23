<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
//import RouteNavBar from '../components/RouteNavBar.vue';
import ErrorModal from '../../../shared/ErrorModal.vue';
import SuccessModal from '../../../shared/SuccessModal.vue';
import AppButton from '../../../shared/AppButton.vue';
import AppInput from '../../../shared/AppInput.vue';
import AppAutocomplete from '../../../shared/AppAutocomplete.vue';
import { useRouteForm } from '../composable/useRouteForm.js';

const {
  daysOfWeek,
  form,
  errors,
  isLoading,
  showSuccessModal,
  successMessage,
  showErrorModal,
  errorMessage,
  modalErrors,
  originSuggestions,
  destinationSuggestions,
  getSuggestionCode,
  getSuggestionLabel,
  handleAirportInput,
  applySuggestion,
  submit,
  onSuccessClose,
} = useRouteForm();
</script>

<template>
  <div class="flex flex-col">
    <PublicNavBar />
    <RouteNavBar/>
    <main class="flex-1 pb-8">
      <div class="page-shell">
        <form class="form-card" @submit.prevent="submit">
          <div class="form-grid">
            <AppAutocomplete
              v-model="form.origin"
              label="Aeropuerto origen"
              :error="errors.fields.origin"
              :suggestions="originSuggestions"
              maxlength="40"
              @update:model-value="handleAirportInput('origin')"
              @select="applySuggestion('origin', $event)"
            >
              <template #suggestion="{ suggestion }">
                <span class="suggestion-code">{{ getSuggestionCode(suggestion) }}</span>
                <span class="suggestion-label">{{ getSuggestionLabel(suggestion) }}</span>
              </template>
            </AppAutocomplete>

            <AppAutocomplete
              v-model="form.destination"
              label="Aeropuerto destino"
              :error="errors.fields.destination"
              :suggestions="destinationSuggestions"
              maxlength="40"
              @update:model-value="handleAirportInput('destination')"
              @select="applySuggestion('destination', $event)"
            >
              <template #suggestion="{ suggestion }">
                <span class="suggestion-code">{{ getSuggestionCode(suggestion) }}</span>
                <span class="suggestion-label">{{ getSuggestionLabel(suggestion) }}</span>
              </template>
            </AppAutocomplete>

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

            <AppInput
              v-model="form.duration"
              label="Duracion del vuelo (min)"
              type="number"
              min="1"
              max="1140"
              step="1"
              :error="errors.fields.duration"
            />

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
            <AppButton type="submit" variant="primary" :loading="isLoading">Guardar ruta</AppButton>
          </div>
        </form>
      </div>
    </main>

    <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="modalErrors" />
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

</style>