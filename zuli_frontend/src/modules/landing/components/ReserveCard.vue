<template>
<<<<<<< HEAD
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
=======
    <div class="reserve-card">
        <ul class="reserve-card__tabs">
            <li>
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
<<<<<<< HEAD
        <div class="flex flex-wrap items-center gap-6 px-8 pt-4 text-sm text-slate-700">
            <label class="inline-flex items-center gap-2">
                <input v-model="selectedFlightOption" type="radio" name="flight-option" value="direct" class="accent-[#711717]" />
                Vuelo directo
            </label>
            <label class="inline-flex items-center gap-2">
                <input v-model="selectedFlightOption" type="radio" name="flight-option" value="layover" class="accent-[#711717]" />
=======
        <div class="reserve-card__flight-options">
            <label class="reserve-card__flight-option">
                <input
                    v-model="selectedFlightOption"
                    type="radio"
                    name="flight-option"
                    value="direct"
                />
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
<<<<<<< HEAD
        <div class="px-8 py-8">
            <div class="grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(0,2fr)_10rem_7rem_minmax(0,1.8fr)] lg:items-end">
                <LandingDropdownField
                    v-model="fromCity"
                    label="Desde"
                    placeholder="Ciudad..."
                    :options="fromCityOptions"
                    button-label="Mostrar ciudades"
                    :container-classes="fromContainerClasses"
                />

                <LandingDropdownField
                    v-model="toCity"
                    label="Hacia"
                    placeholder="Ciudad..."
                    :options="toCityOptions"
                    button-label="Mostrar ciudades"
                    :container-classes="toContainerClasses"
                />

                <div class="flex min-w-0 flex-col gap-1">
                    <label class="text-xs font-semibold text-slate-700">Salida</label>
                    <input
                        v-model="departureDate"
                        type="date"
                        :class="departureDateClass"
                    />
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
                    <input
                        v-model="returnDate"
                        type="date"
                        :class="returnDateClass"
                    />
                </div>
            </div>

            <div class="mt-6 flex justify-end">
                <button type="button" @click="validateAndSearch" class="rounded-md bg-[#711717] px-4 py-2 text-sm font-semibold text-white transition hover:bg-[#5f1313]">
                    Buscar vuelos
                </button>
=======
        <div class="reserve-card__content">
            <div class="reserve-card__form">
                <div class="form-group">
                    <label>Desde</label>
                    <div class="from-city-picker">
                        <div class="from-city-picker__control">
                            <input
                                v-model="fromCity"
                                type="text"
                                class="form-input from-city-picker__input"
                                :class="{ 'form-input--selected': fromCity }"
                                placeholder="Ciudad..."
                                @focus="showFromCityMenu = true"
                            />
                            <button
                                type="button"
                                class="from-city-picker__toggle"
                                aria-label="Mostrar ciudades"
                                @click="toggleFromCityMenu"
                            >
                                <img
                                    class="from-city-picker__arrow"
                                    :src="arrowDownUrl"
                                    alt=""
                                />
                            </button>
                        </div>

                        <div v-if="showFromCityMenu" class="from-city-picker__menu">
                            <button
                                v-for="city in fromCityOptions"
                                :key="city"
                                type="button"
                                class="from-city-picker__menu-item"
                                @click="selectFromCity(city)"
                            >
                                {{ city }}
                            </button>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label>Hacia</label>
                    <div class="to-city-picker">
                        <div class="to-city-picker__control">
                            <input
                                v-model="toCity"
                                type="text"
                                class="form-input to-city-picker__input"
                                :class="{ 'form-input--selected': toCity }"
                                placeholder="Ciudad..."
                                @focus="showToCityMenu = true"
                            />
                            <button
                                type="button"
                                class="to-city-picker__toggle"
                                aria-label="Mostrar ciudades"
                                @click="toggleToCityMenu"
                            >
                                <img
                                    class="to-city-picker__arrow"
                                    :src="arrowDownUrl"
                                    alt=""
                                />
                            </button>
                        </div>

                        <div v-if="showToCityMenu" class="to-city-picker__menu">
                            <button
                                v-for="city in toCityOptions"
                                :key="city"
                                type="button"
                                class="to-city-picker__menu-item"
                                @click="selectToCity(city)"
                            >
                                {{ city }}
                            </button>
                        </div>
                    </div>
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
    import arrowDownUrl from '../../../assets/ArrowDown.svg'

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

<style scoped>
/* Card base */
.reserve-card {
    width: min(100%, 980px);
    background-color: #fff;
    border-radius: 16px;
    box-shadow: 0 12px 30px rgba(0, 0, 0, 0.12);
    overflow: hidden;
    box-sizing: border-box;
}

/* Top tabs */
.reserve-card__tabs {
    display: flex;
    width: 100%;
    background-color: #711717;
    margin: 0px;
    padding: 0px;
    list-style: none;
}

.reserve-card__tab {
  width: 100%;
  padding: 10px;
  border: 0;
  background: transparent;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.reserve-card__tabs li {
  flex: 1;
}

.reserve-card__tabs li + li {
  border-left: 1px solid rgba(255, 255, 255, 0.45);
}


.reserve-card__tab.is-active {
  background-color: rgba(0, 0, 0, 0.25);
}

/* Flight type radio options */
.reserve-card__flight-options {
    display: flex;
    gap: 1.5rem;
    align-items: center;
    padding: 1rem 2rem 0;
}

.reserve-card__flight-option {
    display: inline-flex;
    align-items: center;
    gap: 0.45rem;
    font-size: 0.9rem;
    color: #333;
}

/* Form layout */
.reserve-card__content {
  padding: 2rem;
}

.reserve-card__form {
  display: flex;
    gap: 1rem;
  align-items: flex-end;
}

.reserve-card__button {
    display: flex;
    justify-content: flex-end;
    margin-top: 1rem;
}

/* Shared field sizes */
.form-group {
  display: flex;
  flex-direction: column;
  flex: 1;
    min-width: 140px;
}

.form-group--seats {
    flex: 0 0 95px;
    min-width: 95px;
}

/* Shared picker styles (Desde, Hacia, Asientos, Clase) */
.from-city-picker,
.to-city-picker,
.seats-picker,
.class-picker {
    position: relative;
}

.from-city-picker__control,
.to-city-picker__control,
.seats-picker__control,
.class-picker__control {
    display: flex;
    width: 100%;
    align-items: center;
    border: 1px solid #ddd;
    border-radius: 4px;
    overflow: hidden;
    background-color: #f7f7f7;
}

.from-city-picker__input,
.to-city-picker__input,
.seats-picker__input,
.class-picker__input {
    border: 0;
    border-radius: 0;
    flex: 1;
    min-width: 0;
    background-color: #f7f7f7;
    color: #8b8b8b;
}

.from-city-picker__input,
.to-city-picker__input,
.seats-picker__input {
    cursor: text;
}

.class-picker__input {
    cursor: pointer;
    color: #000000;
}

.from-city-picker__input::placeholder,
.to-city-picker__input::placeholder,
.seats-picker__input::placeholder,
.class-picker__input::placeholder {
    color: #a8a8a8;
    opacity: 1;
}

.from-city-picker__input:focus,
.to-city-picker__input:focus,
.seats-picker__input:focus,
.class-picker__input:focus {
    outline: none;
}

.from-city-picker__toggle,
.to-city-picker__toggle,
.class-picker__toggle,
.seats-picker__toggle {
    border: 0;
    border-left: 1px solid #ddd;
    background-color: #f7f7f7;
    cursor: pointer;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
}

.from-city-picker__toggle,
.to-city-picker__toggle,
.class-picker__toggle {
    width: 42px;
    flex: 0 0 42px;
}

.seats-picker__toggle {
    width: 34px;
    flex: 0 0 34px;
}

.from-city-picker__arrow,
.to-city-picker__arrow,
.class-picker__arrow {
    width: 14px;
    height: 14px;
}

.seats-picker__arrow {
    width: 12px;
    height: 12px;
}

.from-city-picker__menu,
.to-city-picker__menu,
.class-picker__menu,
.seats-picker__menu {
    position: absolute;
    top: calc(100% + 4px);
    left: 0;
    right: 0;
    z-index: 10;
    background-color: #f7f7f7;
    border: 1px solid #ddd;
    border-radius: 6px;
    box-shadow: 0 8px 18px rgba(0, 0, 0, 0.12);
}

.from-city-picker__menu,
.to-city-picker__menu,
.class-picker__menu {
    padding: 0.2rem;
}

.seats-picker__menu {
    max-height: 80px;
    overflow-y: auto;
    padding: 0.1rem;
}

.from-city-picker__menu-item,
.to-city-picker__menu-item,
.class-picker__menu-item,
.seats-picker__menu-item {
    width: 100%;
    border: 0;
    background: #f7f7f7;
    border-radius: 4px;
    cursor: pointer;
    color: #000000;
}

.from-city-picker__menu-item,
.to-city-picker__menu-item,
.class-picker__menu-item {
    text-align: left;
    padding: 0.45rem 0.5rem;
}

.seats-picker__menu-item {
    text-align: center;
    padding: 0.35rem;
}

.from-city-picker__menu-item:hover,
.to-city-picker__menu-item:hover,
.class-picker__menu-item:hover,
.seats-picker__menu-item:hover {
    background-color: #a5a5a5;
}

.seats-picker__input {
    text-align: center;
    text-align-last: center;
}

/* Shared inputs */
.form-input {
  padding: 0.6rem 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.85rem;
    background-color: #f7f7f7;
    color: #8b8b8b;
}

.form-input::placeholder {
    color: #a8a8a8;
    opacity: 1;
}

.form-input--selected {
    color: #000000;
}

.class-picker__input.form-input {
    color: #000000;
    -webkit-text-fill-color: #000000;
}

/* Date input icon */
input[type="date"].form-input::-webkit-calendar-picker-indicator {
    opacity: 1;
    cursor: pointer;
    filter: contrast(1.2) brightness(0.55);
}

input[type="date"].form-input {
    color-scheme: light;
}

/* Labels and CTA */
.form-group label {
  font-weight: 600;
  margin-bottom: 0.3rem;
  font-size: 0.8rem;
  color: #333;
}

.btn-search {
  background-color: #711717;
  color: #fff;
  padding: 0.6rem 1rem;
  border: 0;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  height: fit-content;
  white-space: nowrap;
}
</style>