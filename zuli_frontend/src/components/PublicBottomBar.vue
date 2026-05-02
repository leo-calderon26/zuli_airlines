<template>
  <footer class="fixed bottom-0 left-0 z-20 w-full p-4 bg-primary border-t border-default shadow-sm md:flex md:items-center md:justify-between md:p-6">
      <span class="text-sm text-body sm:text-center">© 2026 <a href="https://flowbite.com/" class="hover:underline">Zuli airlines™</a>. Todos los derechos reservados.
      </span>
      <ul class="flex flex-wrap items-center mt-3 text-sm font-medium text-body sm:mt-0">
          <li>
              <a href="#" class="hover:underline me-4 md:me-6">About</a>
          </li>
          <li>
              <a href="#" class="hover:underline me-4 md:me-6">Privacy Policy</a>
          </li>
          <li>
              <a href="#" class="hover:underline me-4 md:me-6">Licensing</a>
          </li>
          <li>
              <a href="#" class="hover:underline">Contact</a>
          </li>
      </ul>
  </footer>
</template>

<script>
export default {}
</script>

<style scoped>
.public-bottom-bar {
  width: 100%;
  background-color: #711717;
  color: white;
  padding: 18px 24px;
  box-sizing: border-box;
}

.bottom-content {
  display: flex;
  justify-content: space-around;
  align-items: center;
  gap: 20px;
  font-weight: 600;
}

@media (max-width: 768px) {
  .bottom-content {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>