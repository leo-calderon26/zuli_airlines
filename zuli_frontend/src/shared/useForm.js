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

  function onError(error, fallback) {
    const data = error?.response?.data || error?.data
    const msg = data?.detail
      || data?.message
      || error?.message
      || fallback
      || 'Ocurrió un error al procesar la solicitud'
    errorMessage.value = msg
    showErrorModal.value = true

    const backendErrors = data?.errors
    if (backendErrors) {
      for (const [field, msgs] of Object.entries(backendErrors)) {
        const fieldName = field.charAt(0).toLowerCase() + field.slice(1)
        errors.fields[fieldName] = Array.isArray(msgs) ? msgs[0] : msgs
      }
    }
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
