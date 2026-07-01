import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import {
  addAdditionalBaggage,
  getAdditionalBaggageConfirmation,
} from '../service/additionalBaggageService'

const MAX_CARRY_ON = 1
const MAX_CHECKED_BAGS = 5

function getRouteReservationCode(value) {
  const reservationCode = Array.isArray(value) ? value[0] : value
  return String(reservationCode ?? '').trim().toUpperCase()
}

function getField(source, camelCase, pascalCase, fallback = '') {
  return source?.[camelCase] ?? source?.[pascalCase] ?? fallback
}

function formatDateTime(value) {
  if (!value) return ''

  return new Intl.DateTimeFormat('en-US', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  }).format(new Date(value))
}

function formatDuration(start, end) {
  if (!start || !end) return ''

  const minutes = Math.max(Math.round((new Date(end) - new Date(start)) / 60000), 0)
  const hours = Math.floor(minutes / 60)
  const remainingMinutes = minutes % 60

  return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`
}

function getErrorMessage(error) {
  const errors = error.data?.errors

  if (errors) {
    const messages = Object.values(errors)
      .flatMap((value) => Array.isArray(value) ? value : [value])
      .filter(Boolean)

    if (messages.length > 0) return messages[0]
  }

  return error.data?.detail || error.data?.message || error.message || 'No se pudo agregar el equipaje adicional.'
}

export function useAdditionalBaggage() {
  const route = useRoute()
  const isLoading = ref(false)
  const isPaying = ref(false)
  const error = ref(null)
  const success = ref(null)
  const paymentConfirmation = ref(null)
  const trip = ref({
    origin: '',
    destination: '',
    routeLabel: '',
    reservationCode: getRouteReservationCode(route.query.reservationCode),
    totalPaid: 0,
  })
  const passengers = ref([])

  function buildCheckedBagPrices(flights) {
    return Array.from({ length: MAX_CHECKED_BAGS }, (_, index) => flights.reduce((total, flight) => {
      const checkedPrice = getField(flight, 'checkedPrice', 'CheckedPrice', 0)
      const multiplier = getField(flight, 'checkedBagMultiplier', 'CheckedBagMultiplier', 1)
      return total + checkedPrice * Math.pow(multiplier, index)
    }, 0))
  }

  function mapConfirmation(confirmation) {
    const flights = getField(confirmation, 'flights', 'Flights', [])
    const firstFlight = flights[0] ?? {}
    const lastFlight = flights[flights.length - 1] ?? {}
    const checkedBagPrices = buildCheckedBagPrices(flights)
    const carryOnPrice = flights.reduce((total, flight) => (
      total + getField(flight, 'carryOnPrice', 'CarryOnPrice', 0)
    ), 0)

    trip.value = {
      origin: getField(firstFlight, 'originAirportCode', 'OriginAirportCode'),
      destination: getField(lastFlight, 'destinationAirportCode', 'DestinationAirportCode'),
      originName: getField(firstFlight, 'originAirportName', 'OriginAirportName'),
      destinationName: getField(lastFlight, 'destinationAirportName', 'DestinationAirportName'),
      routeLabel: `${getField(firstFlight, 'originAirportName', 'OriginAirportName')} a ${getField(lastFlight, 'destinationAirportName', 'DestinationAirportName')}`.trim(),
      reservationCode: getField(confirmation, 'reservationCode', 'ReservationCode'),
      departureDateText: formatDateTime(getField(firstFlight, 'departureDateTime', 'DepartureDateTime')),
      arrivalDateText: formatDateTime(getField(lastFlight, 'arrivalDateTime', 'ArrivalDateTime')),
      flightNumber: getField(firstFlight, 'flightNumber', 'FlightNumber'),
      airlineName: getField(firstFlight, 'airlineName', 'AirlineName'),
      durationText: formatDuration(
        getField(firstFlight, 'departureDateTime', 'DepartureDateTime'),
        getField(lastFlight, 'arrivalDateTime', 'ArrivalDateTime')
      ),
      totalPaid: getField(confirmation, 'totalAmount', 'TotalAmount', 0),
    }

    passengers.value = getField(confirmation, 'passengers', 'Passengers', []).map((passenger, index) => {
      const passengerId = getField(passenger, 'passengerId', 'PassengerId', null)
        ?? getField(passenger, 'pasangerId', 'PasangerId', null)
      const carryOnQuantity = getField(passenger, 'carryOnQuantity', 'CarryOnQuantity', 0)
      const checkedBaggageQuantity = getField(passenger, 'checkedBaggageQuantity', 'CheckedBaggageQuantity', 0)

      return {
        id: passengerId ?? index + 1,
        passengerId,
        name: getField(passenger, 'fullName', 'FullName'),
        carryOn: carryOnQuantity,
        originalCarryOn: carryOnQuantity,
        checkedBags: checkedBaggageQuantity,
        originalCheckedBags: checkedBaggageQuantity,
        carryOnPrice,
        checkedBagPrices,
      }
    })
  }

  async function loadAdditionalBaggage() {
    if (!trip.value.reservationCode) {
      error.value = 'No se encontró el código de reserva.'
      return
    }

    isLoading.value = true
    error.value = null

    try {
      const confirmation = await getAdditionalBaggageConfirmation(trip.value.reservationCode)
      mapConfirmation(confirmation)
    } catch (err) {
      error.value = getErrorMessage(err)
    } finally {
      isLoading.value = false
    }
  }

  onMounted(loadAdditionalBaggage)

  const passengerTotals = computed(() => passengers.value.map((passenger) => {
    const carryOnTotal = Math.max(passenger.carryOn - passenger.originalCarryOn, 0) * passenger.carryOnPrice
    const checkedTotal = passenger.checkedBagPrices
      .slice(passenger.originalCheckedBags, passenger.checkedBags)
      .reduce((total, price) => total + price, 0)

    return {
      passengerId: passenger.id,
      carryOnTotal,
      checkedTotal,
      total: carryOnTotal + checkedTotal,
    }
  }))

  const totalAmount = computed(() => passengerTotals.value.reduce((sum, item) => sum + item.total, 0))
  const hasChanges = computed(() => passengers.value.some((passenger) => (
    passenger.checkedBags > passenger.originalCheckedBags ||
    passenger.carryOn > passenger.originalCarryOn
  )))

  const selectedItems = computed(() => passengers.value.flatMap((passenger) => {
    const items = []

    if (passenger.carryOn > passenger.originalCarryOn) {
      items.push({
        passengerId: passenger.id,
        passengerName: passenger.name,
        label: `Equipaje de mano x${passenger.carryOn - passenger.originalCarryOn}`,
        price: (passenger.carryOn - passenger.originalCarryOn) * passenger.carryOnPrice,
      })
    }

    passenger.checkedBagPrices.slice(passenger.originalCheckedBags, passenger.checkedBags).forEach((price, index) => {
      items.push({
        passengerId: passenger.id,
        passengerName: passenger.name,
        label: `Maleta documentada #${passenger.originalCheckedBags + index + 1}`,
        price,
      })
    })

    return items
  }))

  function updateBaggage(passengerId, field, direction) {
    passengers.value = passengers.value.map((passenger) => {
      if (passenger.id !== passengerId) return passenger

      const max = field === 'carryOn' ? MAX_CARRY_ON : MAX_CHECKED_BAGS
      const min = field === 'carryOn' ? passenger.originalCarryOn : passenger.originalCheckedBags
      const nextValue = Math.min(max, Math.max(min, passenger[field] + direction))

      return { ...passenger, [field]: nextValue }
    })
  }

  function getPassengerTotal(passengerId) {
    return passengerTotals.value.find((item) => item.passengerId === passengerId)?.total ?? 0
  }

  function buildAdditionalBaggagePayload() {
    return {
      reservationCode: trip.value.reservationCode,
      passengers: passengers.value
        .map((passenger) => ({
          passengerId: passenger.passengerId,
          additionalCheckedBaggage: Math.max(passenger.checkedBags - passenger.originalCheckedBags, 0),
          additionalCarryOn: Math.max(passenger.carryOn - passenger.originalCarryOn, 0),
        }))
        .filter((passenger) => passenger.additionalCheckedBaggage > 0 || passenger.additionalCarryOn > 0),
    }
  }

  async function payAdditionalBaggage() {
    if (!hasChanges.value) {
      error.value = 'Debe agregar al menos un equipaje adicional.'
      return
    }

    const payload = buildAdditionalBaggagePayload()

    if (payload.passengers.some((passenger) => !passenger.passengerId)) {
      error.value = 'No se pudo identificar uno o más pasajeros para agregar equipaje.'
      return
    }

    isPaying.value = true
    error.value = null
    success.value = null
    const paidAmount = totalAmount.value
    const purchasedItems = selectedItems.value.map((item) => ({ ...item }))

    try {
      await addAdditionalBaggage(payload)
      await loadAdditionalBaggage()
      paymentConfirmation.value = {
        reservationCode: trip.value.reservationCode,
        routeLabel: trip.value.routeLabel,
        origin: trip.value.origin,
        destination: trip.value.destination,
        originName: trip.value.originName,
        destinationName: trip.value.destinationName,
        departureDateText: trip.value.departureDateText,
        arrivalDateText: trip.value.arrivalDateText,
        flightNumber: trip.value.flightNumber,
        airlineName: trip.value.airlineName,
        durationText: trip.value.durationText,
        paidAmount,
        totalPaid: trip.value.totalPaid,
        items: purchasedItems,
      }
      success.value = 'Equipaje adicional agregado correctamente.'
    } catch (err) {
      error.value = getErrorMessage(err)
    } finally {
      isPaying.value = false
    }
  }

  return {
    maxCarryOn: MAX_CARRY_ON,
    maxCheckedBags: MAX_CHECKED_BAGS,
    isLoading,
    isPaying,
    error,
    success,
    paymentConfirmation,
    hasChanges,
    passengers,
    passengerTotals,
    selectedItems,
    totalAmount,
    trip,
    getPassengerTotal,
    updateBaggage,
    payAdditionalBaggage,
  }
}
