<script setup>
import HeaderComponent from '../../../shared/HeaderComponent.vue';
import AircraftComponent from '../components/AircraftComponent.vue';
</script>

<template>
    <HeaderComponent/>
    <div class="flex">
    <div class="size-14 grow-2">Esto es para centrar el contenido con box</div>
    <div class="size-14 grow-9">
        <AircraftComponent/>
    </div>
    <div class="size-14 grow-2">Esto es para centrar el contenido con box</div>
    </div>

</template>