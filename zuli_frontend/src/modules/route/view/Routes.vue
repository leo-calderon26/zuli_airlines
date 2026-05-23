<script setup>
import { onMounted } from 'vue';
import PublicNavBar from '../../../shared/PublicNavBar.vue';
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
  aircraftList,
  aircraftLoading,
  getSuggestionCode,
  getSuggestionLabel,
  handleAirportInput,
  applySuggestion,
  submit,
  onSuccessClose,
  fetchAircraftList,
} = useRouteForm();

onMounted(() => {
  fetchAircraftList();
});
</script>

<template>
  <div class="flex flex-col min-h-screen bg-[#F7F3F2]">
    <PublicNavBar />
    <main class="flex-1 w-full max-w-5xl mx-auto py-10 px-4">
      <form @submit.prevent="submit" class="space-y-8">
        <div class="border border-border-soft bg-text-box p-8">
          <h3 class="text-2xl font-bold text-font mb-6">Información de la Ruta</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
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

            <div>
              <label class="mb-2 block text-sm font-medium text-content">Hora de salida</label>
              <input
                v-model="form.departureTime"
                type="time"
                step="60"
                class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 placeholder:text-font/40 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
                :class="{ 'border-error text-error': errors.fields.departureTime }"
              />
              <p v-if="errors.fields.departureTime" class="mt-1 text-sm text-error">{{ errors.fields.departureTime }}</p>
            </div>

            <div>
              <label class="mb-2 block text-sm font-medium text-content">Hora de llegada</label>
              <input
                v-model="form.arrivalTime"
                type="time"
                step="60"
                class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 placeholder:text-font/40 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
                :class="{ 'border-error text-error': errors.fields.arrivalTime }"
              />
              <p v-if="errors.fields.arrivalTime" class="mt-1 text-sm text-error">{{ errors.fields.arrivalTime }}</p>
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

            <div>
              <label class="mb-2 block text-sm font-medium text-content">Aeronave</label>
              <select
                v-model="form.aircraftId"
                class="block w-full appearance-none rounded-base border bg-body px-3 py-2.5 text-sm text-content shadow-xs transition-all duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
                :class="{ 'border-error text-error': errors.fields.aircraftId }"
                :disabled="aircraftLoading"
              >
                <option value="" disabled>Seleccione una aeronave</option>
                <option
                  v-for="ac in aircraftList"
                  :key="ac.aircraftId || ac.id"
                  :value="ac.aircraftId || ac.id"
                >{{ ac.model }}</option>
              </select>
              <p v-if="errors.fields.aircraftId" class="mt-1 text-sm text-error">{{ errors.fields.aircraftId }}</p>
              <p v-if="aircraftLoading" class="mt-1 text-xs text-font/60">Cargando aeronaves...</p>
            </div>
          </div>
        </div>

        <div class="border border-border-soft bg-text-box p-8">
          <h3 class="text-2xl font-bold text-font mb-6">Frecuencia semanal</h3>
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
            <label
              v-for="day in daysOfWeek"
              :key="day.value"
              class="flex items-center gap-3 p-3 border border-border-soft rounded-base cursor-pointer transition hover:border-primary"
              :class="{ 'border-primary bg-primary/5': form.frequency.includes(day.value) }"
            >
              <input
                v-model="form.frequency"
                type="checkbox"
                :value="day.value"
                class="h-4 w-4 accent-sumary"
              />
              <span class="text-sm text-font font-medium">{{ day.label }}</span>
            </label>
          </div>
          <p v-if="errors.fields.frequency" class="mt-3 text-sm text-error">{{ errors.fields.frequency }}</p>
        </div>

        <div class="border border-border-soft bg-text-box p-8">
          <h3 class="text-2xl font-bold text-font mb-6">Precios</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <AppInput
              v-model="form.touristPrice"
              label="Precio Turista ($)"
              type="number"
              min="0"
              step="0.01"
              placeholder="0.00"
              :error="errors.fields.touristPrice"
            />
            <AppInput
              v-model="form.firstClassPrice"
              label="Precio Primera Clase ($)"
              type="number"
              min="0"
              step="0.01"
              placeholder="0.00"
              :error="errors.fields.firstClassPrice"
            />
            <AppInput
              v-model="form.carryOnPrice"
              label="Precio Equipaje de Mano ($)"
              type="number"
              min="0"
              step="0.01"
              placeholder="0.00"
              :error="errors.fields.carryOnPrice"
            />
            <AppInput
              v-model="form.checkedPrice"
              label="Precio Equipaje Documentado ($)"
              type="number"
              min="0"
              step="0.01"
              placeholder="0.00"
              :error="errors.fields.checkedPrice"
            />
          </div>
        </div>

        <div class="flex justify-end">
          <AppButton type="submit" variant="primary" size="lg" :loading="isLoading">
            {{ isLoading ? 'Guardando...' : 'Guardar ruta' }}
          </AppButton>
        </div>
      </form>
    </main>

    <SuccessModal v-model="showSuccessModal" :message="successMessage" @close="onSuccessClose" />
    <ErrorModal v-model="showErrorModal" :message="errorMessage" :errors="modalErrors" />
  </div>
</template>

<style scoped>
@reference "../../../style.css";

.suggestion-code {
  @apply font-semibold text-gray-900;
}

.suggestion-label {
  @apply text-gray-600;
}
</style>
