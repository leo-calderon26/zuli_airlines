<script setup>
    import { onMounted, ref } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import PublicNavBar from '../components/PublicNavBar.vue';
    import FlightCard from '../components/FlightCard.vue';
    import AppButton from '../../../shared/AppButton.vue';
    import ErrorModal from '../../../shared/ErrorModal.vue';
    import { useFlightSearchStore } from '../store/flightSearchStore';

    const route = useRoute();
    const router = useRouter();
    const searchStore = useFlightSearchStore();

    const selectedDepartureFlight = ref(null);
    const selectedDepartureFlightRoutes = ref([]);
    const showUnavailableModal = ref(false);
    const unavailableMessage = ref('');

    const onUnavailableModalClose = async () => {
        showUnavailableModal.value = false;
        await searchStore.performSearch(searchStore.searchParams);
    };

    onMounted(() => {
        const params = {
            Origin: route.query.origin,
            Destination: route.query.destination,
            Date: route.query.date,
            Seats: parseInt(route.query.seats) || 1,
            IsRoundTrip: route.query.roundTrip === 'true',
            ReturnDate: route.query.returnDate || null,
            DirectFlightsOnly: route.query.directOnly === 'true',
            FlightClass: route.query.flightClass || 'Turista',
            Page: 1,
            PageSize: 2
        };

        searchStore.performSearch(params);
    });

    const handleSelectDeparture = async (selection) => {
        const result = await searchStore.verifyFlightAvailability(
            selection.flight,
            searchStore.searchParams.Seats
        );

        const flightRouteSegments = await searchStore.getFlightRouteData(selection.flight);

        if (!result.isAvailable && !result.isError) {
            unavailableMessage.value = "Ya no hay espacios suficientes disponibles para el vuelo de ida seleccionado.";
            showUnavailableModal.value = true;
            return;
        }

        if (result.isError) {
            return;
        }

        if (!searchStore.searchParams.IsRoundTrip) {
            router.push({
                name: 'buyTicket',
                query: {
                    flightData: JSON.stringify({
                        flight: selection.flight,
                        flightClass: selection.travelClass
                    }),
                    seats: searchStore.searchParams.Seats,
                    flightRouteData: JSON.stringify({
                        flightRouteSegments
                    })
                }
            });

            return;
        }

        selectedDepartureFlight.value = selection;
        selectedDepartureFlightRoutes.value = flightRouteSegments;
        changePage(1);
    };

    const clearDepartureSelection = () => {
        selectedDepartureFlight.value = null;
        selectedDepartureFlightRoutes.value = [];
        changePage(1);
    };

    const handleSelectReturn = async (selection) => {
        const result = await searchStore.verifyFlightAvailability(
            selection.flight,
            searchStore.searchParams.Seats
        );

        const returnFlightRouteSegments = await searchStore.getFlightRouteData(selection.flight);

        if (!result.isAvailable && !result.isError) {
            unavailableMessage.value = "Ya no hay espacios suficientes disponibles para el vuelo de regreso seleccionado.";
            showUnavailableModal.value = true;
            return;
        }

        if (result.isError) {
            return;
        }

        router.push({
            name: 'buyTicket',
            query: {
                flightData: JSON.stringify({
                    flight: selectedDepartureFlight.value.flight,
                    flightClass: selectedDepartureFlight.value.travelClass
                }),
                seats: searchStore.searchParams.Seats,
                roundTrip: 'true',
                returnFlight: JSON.stringify(selection.flight),
                flightRouteData: JSON.stringify({
                    flightRouteSegments: selectedDepartureFlightRoutes.value
                }),
                returnFlightRouteData: JSON.stringify({
                    flightRouteSegments: returnFlightRouteSegments
                })
            }
        });
    };

    const changePage = (page) => {
        searchStore.changePage(page);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
</script>