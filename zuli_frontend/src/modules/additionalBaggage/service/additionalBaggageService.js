const ADDITIONAL_BAGGAGE_API_URL = '/api/Baggage'
const PURCHASE_CONFIRMATION_API_URL = '/api/PurchaseConfirmation'

function normalizeReservationCode(reservationCode) {
  return String(reservationCode ?? '').trim().toUpperCase()
}

async function parseErrorResponse(response) {
  const contentType = response.headers.get('content-type') || ''

  if (contentType.includes('application/json') || contentType.includes('+json')) {
    return response.json()
  }

  return { detail: await response.text() }
}

export const addAdditionalBaggage = async (payload) => {
  const normalizedPayload = {
    ...payload,
    reservationCode: normalizeReservationCode(payload.reservationCode),
  }

  const response = await fetch(`${ADDITIONAL_BAGGAGE_API_URL}/additional`, {
    method: 'POST',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(normalizedPayload),
  })

  if (!response.ok) {
    const errorData = await parseErrorResponse(response)
    const error = new Error(errorData.detail || errorData.message || 'No se pudo agregar el equipaje adicional.')
    error.data = errorData
    throw error
  }

  if (response.status === 204) {
    return null
  }

  const contentType = response.headers.get('content-type') || ''
  return contentType.includes('application/json') ? response.json() : null
}

export const getAdditionalBaggageConfirmation = async (reservationCode) => {
  const normalizedReservationCode = normalizeReservationCode(reservationCode)

  if (!normalizedReservationCode) {
    throw new Error('No se encontró el código de reserva.')
  }

  const response = await fetch(`${PURCHASE_CONFIRMATION_API_URL}/${encodeURIComponent(normalizedReservationCode)}`, {
    method: 'GET',
    credentials: 'same-origin',
  })

  if (!response.ok) {
    const errorData = await parseErrorResponse(response)
    const error = new Error(errorData.detail || errorData.message || 'No se pudo cargar la información de equipaje.')
    error.data = errorData
    throw error
  }

  return response.json()
}
