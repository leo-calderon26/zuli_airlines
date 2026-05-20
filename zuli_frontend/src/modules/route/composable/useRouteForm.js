import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { createFlightRoute, searchAirportSuggestionsByName } from '../service/routeService';
import { useForm } from '../../../shared/useForm.js';
import {
  daysOfWeek,
  airportCodePattern,
  BUSINESS_ID_COOKIE,
  AIRLINE_ID,
  normalizeCode,
  normalizeTerm,
  getSuggestionCode,
  getSuggestionLabel,
  isIntegerLike,
  getBusinessId,
  isValidDateTime,
  toIsoDateTime,
  buildDateTimeFromParts,
  encodeDays,
  modalFieldLabels,
  getBackendErrorMessages,
  mapBackendFieldToFormField,
} from '../helpers/routeFormHelper.js';

export function useRouteForm() {
  const router = useRouter();

  const { showSuccessModal, successMessage, showErrorModal, errorMessage, isLoading, errors, clearErrors, onSuccess } = useForm();

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

  const modalErrors = reactive({});

  const originSuggestions = ref([]);
  const destinationSuggestions = ref([]);
  let originSearchTimer = null;
  let destinationSearchTimer = null;
  let latestOriginTerm = '';
  let latestDestinationTerm = '';

  function clearSuggestionList(field) {
    if (field === 'origin') {
      originSuggestions.value = [];
      return;
    }
    destinationSuggestions.value = [];
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

  function validate() {
    clearErrors();

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

  function clearModalErrors() {
    Object.keys(modalErrors).forEach((key) => {
      delete modalErrors[key];
    });
  }

  function syncModalErrors(fieldLabels) {
    clearModalErrors();

    if (errors.global) {
      modalErrors.global = errors.global;
    }

    Object.entries(errors.fields).forEach(([field, message]) => {
      modalErrors[fieldLabels[field] || field] = message;
    });
  }

  function applyBackendValidationErrors(errorPayload) {
    const backendErrors = errorPayload?.errors;
    if (!backendErrors || typeof backendErrors !== 'object') return false;

    errors.fields = {};
    const globalMessages = [];

    Object.entries(backendErrors).forEach(([field, messages]) => {
      const parsedMessages = getBackendErrorMessages(messages);
      if (parsedMessages.length === 0) return;

      const formField = mapBackendFieldToFormField(field);
      if (formField) {
        errors.fields[formField] = parsedMessages[0];
        return;
      }

      globalMessages.push(...parsedMessages);
    });

    errors.global = globalMessages.join(' ');
    return Object.keys(errors.fields).length > 0 || Boolean(errors.global);
  }

  async function submit() {
    if (isLoading.value) return;

    if (!validate()) {
      syncModalErrors(modalFieldLabels);
      errorMessage.value = errors.global || 'Corrige los campos marcados.';
      showErrorModal.value = true;
      return;
    }

    const businessId = getBusinessId();

    if (!businessId) {
      errorMessage.value = 'No se pudo obtener el businessId desde las cookies.';
      showErrorModal.value = true;
      return;
    }

    isLoading.value = true;

    try {
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
        businessId,
        airlineId: AIRLINE_ID,
        arrivalAirport: normalizeCode(form.destination),
        departureAirport: normalizeCode(form.origin),
      };

      await createFlightRoute(routePayload);
      onSuccess('La ruta de vuelo se ha creado correctamente');
    } catch (error) {
      const backendPayload = error?.response?.data;
      const hasAppliedBackendErrors = applyBackendValidationErrors(backendPayload);
      syncModalErrors(modalFieldLabels);

      if (hasAppliedBackendErrors) {
        errorMessage.value = backendPayload?.detail || backendPayload?.message || 'Revisa los campos marcados.';
      } else {
        errorMessage.value = backendPayload?.detail || backendPayload?.message || 'Error al crear la ruta';
      }

      showErrorModal.value = true;
    } finally {
      isLoading.value = false;
    }
  }

  function onSuccessClose() {
    router.push({ name: 'routes' });
  }

  return {
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
  };
}
