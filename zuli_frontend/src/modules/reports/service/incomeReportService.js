const API_URL = '/api/IncomeReport'
const EXPORT_URL = '/api/IncomeReport/export'

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

const getFilenameFromHeader = (header) => {
  if (!header) return null
  const starMatch = header.match(/filename\*=UTF-8''([^;]+)/i)
  if (starMatch) return decodeURIComponent(starMatch[1])
  const match = header.match(/filename="([^"]+)"/)
  if (match) return match[1]
  return null
}

export const exportIncomeReport = async (filters) => {
  const params = new URLSearchParams()
  if (filters.year) params.append('year', filters.year)
  if (filters.origin) params.append('origin', filters.origin)
  if (filters.destination) params.append('destination', filters.destination)
  if (filters.airline) params.append('airlineId', filters.airline)

  const query = params.toString()
  const url = query ? `${EXPORT_URL}?${query}` : EXPORT_URL

  const response = await fetch(url, {
    method: 'GET',
    credentials: 'include'
  })

  if (!response.ok) {
    let data = {}
    try { data = await response.json() } catch { data = {} }
    const err = new Error(data.detail || data.message || 'Error al exportar el reporte de ingresos')
    err.status = response.status
    err.data = data
    throw err
  }

  const blob = await response.blob()
  const filename = getFilenameFromHeader(response.headers.get('content-disposition'))
  return { blob, filename }
}
