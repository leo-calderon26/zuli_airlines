const API_URL = '/api/IncomeReport'

export const getIncomeReport = async (filters) => {
  const params = new URLSearchParams()
  if (filters.year) params.append('year', filters.year)
  if (filters.origin) params.append('origin', filters.origin)
  if (filters.destination) params.append('destination', filters.destination)
  if (filters.airline) params.append('airlineId', filters.airline)

  const query = params.toString()
  const url = query ? `${API_URL}?${query}` : API_URL

  const response = await fetch(url, {
    method: 'GET',
    credentials: 'include'
  })

  const data = await response.json().catch(() => ({}))

  if (!response.ok) {
    const err = new Error(data.detail || data.message || 'Error al obtener el reporte de ingresos')
    err.status = response.status
    err.data = data
    throw err
  }

  return data
}
