<template>
    <div class="w-full max-w-5xl overflow-hidden rounded-2xl bg-white shadow-[0_12px_30px_rgba(0,0,0,0.12)]">
        <ul class="m-0 flex w-full list-none bg-[#711717] p-0">
            <li class="flex-1">
                <button
                    type="button"
                    class="flex w-full items-center justify-center px-3 py-3 text-center text-sm font-semibold text-white transition"
                    :class="activeTab === 'oneWay' ? 'bg-black/25' : 'hover:bg-white/10'"
                    @click="activeTab = 'oneWay'"
                >
                    Viaje solo ida
                </button>
            </li>
            <li class="flex-1">
                <button
                    type="button"
                    class="flex w-full items-center justify-center px-3 py-3 text-center text-sm font-semibold text-white transition"
                    :class="activeTab === 'roundTrip' ? 'bg-black/25' : 'hover:bg-white/10'"
                    @click="activeTab = 'roundTrip'"
                >
                    Viaje ida y vuelta
                </button>
            </li>
            <li class="flex-1">
                <button
                    type="button"
                    class="flex w-full items-center justify-center px-3 py-3 text-center text-sm font-semibold text-white transition"
                    :class="activeTab === 'multiCity' ? 'bg-black/25' : 'hover:bg-white/10'"
                    @click="activeTab = 'multiCity'"
                >
                    Multiciudad
                </button>
            </li>
        </ul>

        <div class="flex flex-wrap items-center gap-6 px-8 pt-4 text-sm text-slate-700">
            <label class="inline-flex items-center gap-2">
                <input v-model="selectedFlightOption" type="radio" name="flight-option" value="direct" class="accent-[#711717]" />
                Vuelo directo
            </label>
            <label class="inline-flex items-center gap-2">
                <input v-model="selectedFlightOption" type="radio" name="flight-option" value="layover" class="accent-[#711717]" />
                Vuelo con escalas
            </label>
        </div>

        <div class="px-8 py-8">
            <div class="grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(0,2fr)_10rem_7rem_minmax(0,1.8fr)] lg:items-end">
                <LandingDropdownField
                    v-model="fromCity"
                    label="Desde"
                    placeholder="Ciudad..."
                    :options="fromCityOptions"
                    button-label="Mostrar ciudades"
                />

                <LandingDropdownField
                    v-model="toCity"
                    label="Hacia"
                    placeholder="Ciudad..."
                    :options="toCityOptions"
                    button-label="Mostrar ciudades"
                />

                <div class="flex min-w-0 flex-col gap-1">
                    <label class="text-xs font-semibold text-slate-700">Salida</label>
                    <input type="date" class="rounded-md border border-slate-200 bg-[#f7f7f7] px-3 py-2 text-sm text-slate-900 focus:outline-none" />
                </div>

                <LandingDropdownField
                    v-model="seatsCount"
                    label="Asientos"
                    :options="seatsOptions"
                    input-classes="min-w-0 flex-1 bg-transparent px-3 py-2 text-center text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none"
                    button-classes="flex w-8 shrink-0 items-center justify-center border-l border-slate-200 bg-[#f7f7f7] transition hover:bg-slate-200"
                    button-label="Mostrar asientos"
                />

                <LandingDropdownField
                    v-model="travelClass"
                    label="Clase"
                    :options="classOptions"
                    :readonly="true"
                    button-label="Mostrar clases"
                />
            </div>

            <div v-if="activeTab === 'roundTrip'" class="mt-4 grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(0,2fr)_10rem_7rem_minmax(0,1.8fr)] lg:items-end">
                <div class="flex min-w-0 flex-col gap-1 lg:col-start-2">
                    <label class="text-xs font-semibold text-slate-700">Regreso</label>
                    <input type="date" class="rounded-md border border-slate-200 bg-[#f7f7f7] px-3 py-2 text-sm text-slate-900 focus:outline-none" />
                </div>
            </div>

            <div class="mt-6 flex justify-end">
                <button class="rounded-md bg-[#711717] px-4 py-2 text-sm font-semibold text-white transition hover:bg-[#5f1313]">
                    Buscar vuelos
                </button>
            </div>
        </div>
    </div>
</template>

<script>
    import LandingDropdownField from './LandingDropdownField.vue'

    export default {
        components: {
            LandingDropdownField
        },
        data() {
            return {
                activeTab: 'oneWay',
                selectedFlightOption: 'direct',
                fromCity: '',
                fromCityOptions: ['San José', 'Liberia'],
                toCity: '',
                toCityOptions: ['New York', 'Miami'],
                seatsCount: '1',
                seatsOptions: Array.from({ length: 30 }, (_, i) => String(i + 1)),
                travelClass: 'Turista',
                classOptions: ['Turista', 'Primera Clase']
            }
        }
    }
</script>
