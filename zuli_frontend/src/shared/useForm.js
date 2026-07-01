import { reactive, ref } from 'vue'

export function useForm() {
  const showSuccessModal = ref(false)
  const successMessage = ref('')
  const showErrorModal = ref(false)
  const errorMessage = ref('')
  const isLoading = ref(false)

  const errors = reactive({
    global: '',
    fields: {},
  })

  function clearErrors() {
    errors.global = ''
    errors.fields = {}
  }

  function onSuccess(message) {
    successMessage.value = message
    showSuccessModal.value = true
  }

  function getFirstBackendError(data) {
    const backendErrors = data?.errors

    if (!backendErrors) {
      return null
    }

    for (const messages of Object.values(backendErrors)) {
      if (Array.isArray(messages) && messages.length > 0) {
        return messages[0]
      }

      if (typeof messages === 'string' && messages.trim()) {
        return messages
      }
    }

    return null
  }

  function onError(error, fallback) {
    const data = error?.response?.data || error?.data

    const msg = getFirstBackendError(data)
      || data?.message
      || data?.detail
      || error?.message
      || fallback
      || 'Ocurrió un error al procesar la solicitud'

    errorMessage.value = msg
    showErrorModal.value = true
  }
    function resetModals() {
      showErrorModal.value = false
      showSuccessModal.value = false
      errorMessage.value = ''
      successMessage.value = ''
    }

  async function handleSubmit(fn, errorFallback) {
    if (isLoading.value) return
    isLoading.value = true
    resetModals()
    clearErrors()
    try {
      await fn()
    } catch (error) {
      onError(error, errorFallback)
    } finally {
      isLoading.value = false
    }
  }

  return {
    showSuccessModal,
    successMessage,
    showErrorModal,
    errorMessage,
    isLoading,
    errors,
    clearErrors,
    onSuccess,
    onError,
    resetModals,
    handleSubmit,
  }
}
