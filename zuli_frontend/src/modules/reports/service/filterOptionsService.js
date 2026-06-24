import axios from 'axios'

const API_URL = '/api/FilterOptions/filteroptions'

export const getFilterOptions = async () => {
  try {
    const response = await axios.get(API_URL)
    return response.data
  } catch (error) {
    console.error('Error al obtener opciones de filtros:', error)
    throw error
  }
}
