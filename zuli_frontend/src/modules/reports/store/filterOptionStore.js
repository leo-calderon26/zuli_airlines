import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getFilterOptions } from '../service/filterOptionsService'

export const useFilterOptionsStore = defineStore('filterOptions', () => {
  const years = ref([])
  const origins = ref([])
  const destinations = ref([])
  const airlines = ref([])
  const classes = ref([])
  const isLoading = ref(false)
  const error = ref(null)
  const isLoaded = ref(false)

  const ensureLoaded = async () => {
    if (isLoaded.value || isLoading.value) return
    isLoading.value = true
    error.value = null
    try {
      const data = await getFilterOptions()
      years.value = data.years ?? []
      origins.value = data.origins ?? []
      destinations.value = data.destinations ?? []
      airlines.value = data.airlines ?? []
      classes.value = data.classes ?? []
      isLoaded.value = true
    } catch (e) {
      error.value = e.response?.data?.detail || e.message || 'Error al cargar las opciones de filtros'
    } finally {
      isLoading.value = false
    }
  }

  return {
    years,
    origins,
    destinations,
    airlines,
    classes,
    isLoading,
    error,
    isLoaded,
    ensureLoaded
  }
})
