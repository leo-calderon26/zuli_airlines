<script setup>
import AircraftComponent from '../components/AircraftComponent.vue';
import HeaderComponent from '../components/HeaderComponent.vue';

</script>

<template>
    <HeaderComponent/>
    <div class="flex">
    <div class="size-14 grow-2">Esto es para centrar el contenido con box</div>
    <div class="size-14 grow-9">
        <AircraftComponent></AircraftComponent>
    </div>
    <div class="size-14 grow-2">Esto es para centrar el contenido con box</div>
    </div>

</template>